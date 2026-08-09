using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
// Domain.Entities defines its own "Task" entity (an unrelated, in-flight parallel module),
// which collides with System.Threading.Tasks.Task now that both namespaces are in scope in
// this file - alias it explicitly so xUnit's async Task test methods keep compiling.
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class ElectionServiceTests
{
    private readonly Mock<IElectionRepository> _electionRepositoryMock;
    private readonly Mock<IElectionAudienceTargetRepository> _audienceTargetRepositoryMock;
    private readonly Mock<IElectionCandidateRepository> _candidateRepositoryMock;
    private readonly Mock<IElectionVoteRepository> _voteRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ElectionService _service;

    public ElectionServiceTests()
    {
        _electionRepositoryMock = new Mock<IElectionRepository>();
        _audienceTargetRepositoryMock = new Mock<IElectionAudienceTargetRepository>();
        _candidateRepositoryMock = new Mock<IElectionCandidateRepository>();
        _voteRepositoryMock = new Mock<IElectionVoteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new ElectionService(
            _electionRepositoryMock.Object,
            _audienceTargetRepositoryMock.Object,
            _candidateRepositoryMock.Object,
            _voteRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    private static Election OpenElection(long electionId = 1, bool allowMultipleChoice = false) => new()
    {
        ElectionId = electionId,
        Title = "Employee of the Year",
        ElectionType = "SingleChoice",
        CandidateType = "Employee",
        AudienceScope = "All",
        Status = "Open",
        AllowMultipleChoice = allowMultipleChoice,
        StartDate = DateTime.UtcNow.AddDays(-1),
        EndDate = DateTime.UtcNow.AddDays(1)
    };

    private static ElectionCandidate ApprovedCandidate(long electionId = 1, long candidateId = 10) => new()
    {
        ElectionCandidateId = candidateId,
        ElectionId = electionId,
        EmployeeId = 5,
        CandidateType = "Employee",
        IsApproved = true,
        NominatedAt = DateTime.UtcNow.AddDays(-2)
    };

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new ElectionCreateRequest
        {
            Title = "Employee of the Year",
            ElectionType = "SingleChoice",
            CandidateType = "Employee",
            AudienceScope = "All",
            StartDate = DateTime.UtcNow
        };

        _electionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Election>()))
            .Callback<Election>(e => e.ElectionId = 1);

        // Act
        var result = await _service.CreateAsync(request, createdBy: 99);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Election?)null);

        // Act
        var act = () => _service.GetByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ApproveCandidateAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _candidateRepositoryMock.Setup(r => r.GetByIdForElectionAsync(1, 10)).ReturnsAsync((ElectionCandidate?)null);

        // Act
        var act = () => _service.ApproveCandidateAsync(1, 10);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ApproveCandidateAsync_WhenFound_ShouldSetIsApprovedTrue()
    {
        // Arrange
        var candidate = new ElectionCandidate { ElectionCandidateId = 10, ElectionId = 1, IsApproved = false };
        _candidateRepositoryMock.Setup(r => r.GetByIdForElectionAsync(1, 10)).ReturnsAsync(candidate);

        // Act
        await _service.ApproveCandidateAsync(1, 10);

        // Assert
        candidate.IsApproved.Should().BeTrue();
        _candidateRepositoryMock.Verify(r => r.Update(candidate), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CastVoteAsync_WhenElectionNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Election?)null);

        // Act
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateId = 10 }, voterId: 5);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CastVoteAsync_WhenElectionNotOpen_ShouldThrowForbiddenException()
    {
        // Arrange
        var election = OpenElection();
        election.Status = "Closed";
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);

        // Act
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateId = 10 }, voterId: 5);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CastVoteAsync_WhenOutsideVotingWindow_ShouldThrowForbiddenException()
    {
        // Arrange
        var election = OpenElection();
        election.StartDate = DateTime.UtcNow.AddDays(1);
        election.EndDate = DateTime.UtcNow.AddDays(2);
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);

        // Act
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateId = 10 }, voterId: 5);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CastVoteAsync_WhenCandidateNotApproved_ShouldThrowForbiddenException()
    {
        // Arrange
        var election = OpenElection();
        var candidate = ApprovedCandidate();
        candidate.IsApproved = false;
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _candidateRepositoryMock.Setup(r => r.GetByIdForElectionAsync(1, 10)).ReturnsAsync(candidate);

        // Act
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateId = 10 }, voterId: 5);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CastVoteAsync_WhenVoterAlreadyVotedAndMultipleChoiceNotAllowed_ShouldThrowDuplicateException()
    {
        // Arrange
        var election = OpenElection(allowMultipleChoice: false);
        var candidate = ApprovedCandidate();
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _candidateRepositoryMock.Setup(r => r.GetByIdForElectionAsync(1, 10)).ReturnsAsync(candidate);
        _voteRepositoryMock.Setup(r => r.HasVotedAsync(1, 5)).ReturnsAsync(true);

        // Act
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateId = 10 }, voterId: 5);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task CastVoteAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var election = OpenElection(allowMultipleChoice: false);
        var candidate = ApprovedCandidate();
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _candidateRepositoryMock.Setup(r => r.GetByIdForElectionAsync(1, 10)).ReturnsAsync(candidate);
        _voteRepositoryMock.Setup(r => r.HasVotedAsync(1, 5)).ReturnsAsync(false);
        _voteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ElectionVote>()))
            .Callback<ElectionVote>(v => v.ElectionVoteId = 42);

        // Act
        var result = await _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateId = 10 }, voterId: 5);

        // Assert
        result.Should().Be(42);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CastVoteAsync_WhenMultipleChoiceAllowed_ShouldSkipDuplicateCheck()
    {
        // Arrange
        var election = OpenElection(allowMultipleChoice: true);
        var candidate = ApprovedCandidate();
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _candidateRepositoryMock.Setup(r => r.GetByIdForElectionAsync(1, 10)).ReturnsAsync(candidate);
        _voteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ElectionVote>()))
            .Callback<ElectionVote>(v => v.ElectionVoteId = 43);

        // Act
        var result = await _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateId = 10 }, voterId: 5);

        // Assert
        result.Should().Be(43);
        _voteRepositoryMock.Verify(r => r.HasVotedAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
    }
}
