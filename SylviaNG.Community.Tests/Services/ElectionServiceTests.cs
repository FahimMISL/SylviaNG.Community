using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
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
    private readonly Mock<IElectionEligibilityService> _eligibilityServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ElectionService _service;

    public ElectionServiceTests()
    {
        _electionRepositoryMock = new Mock<IElectionRepository>();
        _audienceTargetRepositoryMock = new Mock<IElectionAudienceTargetRepository>();
        _candidateRepositoryMock = new Mock<IElectionCandidateRepository>();
        _voteRepositoryMock = new Mock<IElectionVoteRepository>();
        _eligibilityServiceMock = new Mock<IElectionEligibilityService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new ElectionService(
            _electionRepositoryMock.Object,
            _audienceTargetRepositoryMock.Object,
            _candidateRepositoryMock.Object,
            _voteRepositoryMock.Object,
            _eligibilityServiceMock.Object,
            _notificationServiceMock.Object,
            _unitOfWorkMock.Object);

        // Default: any voter is eligible, unless a test overrides this.
        _audienceTargetRepositoryMock.Setup(r => r.GetByElectionIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new List<ElectionAudienceTarget>());
        _eligibilityServiceMock
            .Setup(s => s.GetEligibleEmployeeIdsAsync(It.IsAny<Election>(), It.IsAny<List<ElectionAudienceTarget>>()))
            .ReturnsAsync(new HashSet<long> { 5 });
    }

    private static Election OpenElection(long electionId = 1, bool allowMultipleChoice = false, int minSelection = 1, int maxSelection = 1) => new()
    {
        ElectionId = electionId,
        Title = "Employee of the Year",
        ElectionType = "SingleChoice",
        CandidateType = "Employee",
        AudienceScope = "Organization",
        Status = "Open",
        AllowMultipleChoice = allowMultipleChoice,
        MinSelection = minSelection,
        MaxSelection = maxSelection,
        StartDate = DateTime.UtcNow.AddDays(-1),
        EndDate = DateTime.UtcNow.AddDays(1)
    };

    private static ElectionCandidate NominatedCandidate(long electionId = 1, long candidateId = 10) => new()
    {
        ElectionCandidateId = candidateId,
        ElectionId = electionId,
        EmployeeId = 5,
        CandidateType = "Employee",
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
            AudienceScope = "Organization",
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
    public async Task NominateBulkAsync_WhenElectionNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Election?)null);

        // Act
        var act = () => _service.NominateBulkAsync(1, new ElectionCandidateNominateBulkRequest { Scope = "Organization" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task NominateBulkAsync_WhenCandidateTypeIsNotEmployee_ShouldThrowForbiddenException()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", CandidateType = "Team" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);

        // Act
        var act = () => _service.NominateBulkAsync(1, new ElectionCandidateNominateBulkRequest { Scope = "Organization" });

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task NominateBulkAsync_WhenNoEmployeesResolved_ShouldReturnZeroAndNotSave()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", CandidateType = "Employee" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _eligibilityServiceMock
            .Setup(s => s.ResolveCandidateEmployeeIdsAsync("Department", It.IsAny<List<long>>()))
            .ReturnsAsync(new HashSet<long>());

        // Act
        var result = await _service.NominateBulkAsync(1, new ElectionCandidateNominateBulkRequest { Scope = "Department", TargetIds = new List<long> { 7 } });

        // Assert
        result.Should().Be(0);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task NominateBulkAsync_WhenValid_ShouldSkipAlreadyNominatedEmployeesAndReturnNewCount()
    {
        // Arrange - employee 5 is already nominated; only 6 and 7 should be newly added
        var election = new Election { ElectionId = 1, Title = "T", CandidateType = "Employee" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _eligibilityServiceMock
            .Setup(s => s.ResolveCandidateEmployeeIdsAsync("Organization", It.IsAny<List<long>>()))
            .ReturnsAsync(new HashSet<long> { 5, 6, 7 });
        _candidateRepositoryMock
            .Setup(r => r.GetByElectionIdAsync(1))
            .ReturnsAsync(new List<ElectionCandidate> { NominatedCandidate(candidateId: 10) }); // EmployeeId = 5

        List<ElectionCandidate>? addedCandidates = null;
        _candidateRepositoryMock
            .Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<ElectionCandidate>>()))
            .Callback<IEnumerable<ElectionCandidate>>(c => addedCandidates = c.ToList());

        // Act
        var result = await _service.NominateBulkAsync(1, new ElectionCandidateNominateBulkRequest { Scope = "Organization" });

        // Assert
        result.Should().Be(2);
        addedCandidates.Should().HaveCount(2);
        addedCandidates!.Select(c => c.EmployeeId).Should().BeEquivalentTo(new long?[] { 6, 7 });
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CastVoteAsync_WhenElectionNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Election?)null);

        // Act
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } }, voterId: 5);

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
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } }, voterId: 5);

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
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } }, voterId: 5);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CastVoteAsync_WhenVoterNotEligible_ShouldThrowForbiddenException()
    {
        // Arrange
        var election = OpenElection();
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _eligibilityServiceMock
            .Setup(s => s.GetEligibleEmployeeIdsAsync(It.IsAny<Election>(), It.IsAny<List<ElectionAudienceTarget>>()))
            .ReturnsAsync(new HashSet<long>()); // voter 5 not in the eligible set

        // Act
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } }, voterId: 5);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CastVoteAsync_WhenSelectionCountOutsideMinMax_ShouldThrowForbiddenException()
    {
        // Arrange - multi-choice election requiring 2-3 selections, voter selects only 1
        var election = OpenElection(allowMultipleChoice: true, minSelection: 2, maxSelection: 3);
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);

        // Act
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } }, voterId: 5);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CastVoteAsync_WhenVoterAlreadyVoted_ShouldThrowDuplicateException()
    {
        // Arrange
        var election = OpenElection(allowMultipleChoice: false);
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _voteRepositoryMock.Setup(r => r.HasVotedAsync(1, 5)).ReturnsAsync(true);

        // Act
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } }, voterId: 5);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task CastVoteAsync_WhenVoterAlreadyVotedAndMultipleChoiceAllowed_ShouldStillThrowDuplicateException()
    {
        // Arrange - one ballot per employee per election, regardless of single/multi-choice
        var election = OpenElection(allowMultipleChoice: true, minSelection: 1, maxSelection: 2);
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _voteRepositoryMock.Setup(r => r.HasVotedAsync(1, 5)).ReturnsAsync(true);

        // Act
        var act = () => _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } }, voterId: 5);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task CastVoteAsync_WithValidRequest_ShouldReturnCreatedVoteIds()
    {
        // Arrange
        var election = OpenElection(allowMultipleChoice: false);
        var candidate = NominatedCandidate();
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _candidateRepositoryMock
            .Setup(r => r.GetByIdsForElectionAsync(1, It.Is<IEnumerable<long>>(ids => ids.Contains(10))))
            .ReturnsAsync(new List<ElectionCandidate> { candidate });
        _voteRepositoryMock.Setup(r => r.HasVotedAsync(1, 5)).ReturnsAsync(false);
        _voteRepositoryMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<ElectionVote>>()))
            .Callback<IEnumerable<ElectionVote>>(votes =>
            {
                long id = 42;
                foreach (var v in votes) v.ElectionVoteId = id++;
            });

        // Act
        var result = await _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } }, voterId: 5);

        // Assert
        result.Should().BeEquivalentTo(new List<long> { 42 });
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CastVoteAsync_WhenMultipleChoiceAllowed_ShouldCreateOneVoteRowPerCandidate()
    {
        // Arrange
        var election = OpenElection(allowMultipleChoice: true, minSelection: 1, maxSelection: 2);
        var candidateA = NominatedCandidate(candidateId: 10);
        var candidateB = NominatedCandidate(candidateId: 11);
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _candidateRepositoryMock
            .Setup(r => r.GetByIdsForElectionAsync(1, It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<ElectionCandidate> { candidateA, candidateB });
        _voteRepositoryMock.Setup(r => r.HasVotedAsync(1, 5)).ReturnsAsync(false);

        List<ElectionVote>? capturedVotes = null;
        _voteRepositoryMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<ElectionVote>>()))
            .Callback<IEnumerable<ElectionVote>>(votes =>
            {
                capturedVotes = votes.ToList();
                long id = 1;
                foreach (var v in capturedVotes) v.ElectionVoteId = id++;
            });

        // Act
        var result = await _service.CastVoteAsync(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10, 11 } }, voterId: 5);

        // Assert
        result.Should().HaveCount(2);
        capturedVotes.Should().HaveCount(2);
    }

    [Fact]
    public async Task PublishAsync_WhenScopeNotConfigured_ShouldThrowForbiddenException()
    {
        // Arrange - Department scope with zero audience targets configured
        var election = new Election { ElectionId = 1, Title = "T", AudienceScope = "Department", Status = "Draft", MinSelection = 1, MaxSelection = 1 };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _audienceTargetRepositoryMock.Setup(r => r.GetByElectionIdAsync(1)).ReturnsAsync(new List<ElectionAudienceTarget>());

        // Act
        var act = () => _service.PublishAsync(1);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task PublishAsync_WhenFewerCandidatesThanMinSelection_ShouldThrowForbiddenException()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", AudienceScope = "Organization", Status = "Draft", MinSelection = 2, MaxSelection = 2 };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _candidateRepositoryMock.Setup(r => r.CountAsync(1)).ReturnsAsync(1);

        // Act
        var act = () => _service.PublishAsync(1);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task PublishAsync_WhenValid_ShouldSetStatusOpenAndNotifyEligibleEmployees()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", AudienceScope = "Organization", Status = "Draft", MinSelection = 1, MaxSelection = 1 };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _candidateRepositoryMock.Setup(r => r.CountAsync(1)).ReturnsAsync(1);
        _eligibilityServiceMock
            .Setup(s => s.GetEligibleEmployeeIdsAsync(election, It.IsAny<List<ElectionAudienceTarget>>()))
            .ReturnsAsync(new HashSet<long> { 5, 6 });

        // Act
        await _service.PublishAsync(1);

        // Assert
        election.Status.Should().Be("Open");
        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<Application.Features.Notifications.Models.NotificationCreateRequest>()), Times.Exactly(2));
    }

    [Fact]
    public async Task PublishAsync_WhenAlreadyPublished_ShouldThrowForbiddenException()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", Status = "Open" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);

        // Act
        var act = () => _service.PublishAsync(1);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CloseAsync_WhenOpen_ShouldSetStatusClosed()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", Status = "Open" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);

        // Act
        await _service.CloseAsync(1);

        // Assert
        election.Status.Should().Be("Closed");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CloseAsync_WhenDraft_ShouldThrowForbiddenException()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", Status = "Draft" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);

        // Act
        var act = () => _service.CloseAsync(1);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenDraft_ShouldDelete()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", Status = "Draft" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _electionRepositoryMock.Verify(r => r.Delete(election), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenAlreadyPublished_ShouldThrowForbiddenException()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", Status = "Open" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);

        // Act
        var act = () => _service.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _electionRepositoryMock.Verify(r => r.Delete(It.IsAny<Election>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenNoVotesCast_ShouldApplyFullUpdate()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "Old Title", AudienceScope = "Organization", Status = "Draft" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _voteRepositoryMock.Setup(r => r.HasAnyVotesAsync(1)).ReturnsAsync(false);

        // Act
        await _service.UpdateAsync(1, new ElectionUpdateRequest { Title = "New Title", AudienceScope = "Department" });

        // Assert
        election.Title.Should().Be("New Title");
        election.AudienceScope.Should().Be("Department");
    }

    [Fact]
    public async Task UpdateAsync_WhenVotesCastAndLockedFieldChanged_ShouldThrowForbiddenException()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "Old Title", AudienceScope = "Organization", Status = "Open" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _voteRepositoryMock.Setup(r => r.HasAnyVotesAsync(1)).ReturnsAsync(true);

        // Act
        var act = () => _service.UpdateAsync(1, new ElectionUpdateRequest { AudienceScope = "Department" });

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task UpdateAsync_WhenVotesCastAndOnlyTitleChanged_ShouldApplyRestrictedUpdate()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "Old Title", AudienceScope = "Organization", Status = "Open" };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _voteRepositoryMock.Setup(r => r.HasAnyVotesAsync(1)).ReturnsAsync(true);

        // Act
        await _service.UpdateAsync(1, new ElectionUpdateRequest { Title = "New Title" });

        // Assert
        election.Title.Should().Be("New Title");
        election.AudienceScope.Should().Be("Organization"); // unchanged
    }

    [Fact]
    public async Task UpdateAsync_WhenEndDateAtOrBeforeExistingStartDate_ShouldThrowForbiddenException()
    {
        // Arrange - request only sends EndDate (StartDate left as-is); must still be checked
        // against the election's existing, already-stored StartDate.
        var election = new Election
        {
            ElectionId = 1,
            Title = "T",
            AudienceScope = "Organization",
            Status = "Draft",
            StartDate = new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc)
        };
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _voteRepositoryMock.Setup(r => r.HasAnyVotesAsync(1)).ReturnsAsync(false);

        // Act - new EndDate equals StartDate, which is invalid (must be strictly after)
        var act = () => _service.UpdateAsync(1, new ElectionUpdateRequest { EndDate = election.StartDate });

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetResultsAsync_WhenAnonymous_ShouldNotIncludeVoterDetails()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", IsAnonymous = true, Status = "Closed" };
        var candidate = NominatedCandidate();
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _candidateRepositoryMock.Setup(r => r.GetByElectionIdAsync(1)).ReturnsAsync(new List<ElectionCandidate> { candidate });
        _voteRepositoryMock.Setup(r => r.GetAllForElectionAsync(1)).ReturnsAsync(new List<ElectionVote>
        {
            new() { ElectionVoteId = 1, ElectionId = 1, CandidateId = 10, VoterId = 5, VotedAt = DateTime.UtcNow }
        });

        // Act
        var result = await _service.GetResultsAsync(1);

        // Assert
        result.VoterDetails.Should().BeNull();
        result.CandidateTallies.Should().ContainSingle(t => t.ElectionCandidateId == 10 && t.VoteCount == 1);
        result.TotalVotes.Should().Be(1);
    }

    [Fact]
    public async Task GetResultsAsync_WhenIdentified_ShouldIncludeVoterDetails()
    {
        // Arrange
        var election = new Election { ElectionId = 1, Title = "T", IsAnonymous = false, Status = "Closed" };
        var candidate = NominatedCandidate();
        _electionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(election);
        _candidateRepositoryMock.Setup(r => r.GetByElectionIdAsync(1)).ReturnsAsync(new List<ElectionCandidate> { candidate });
        _voteRepositoryMock.Setup(r => r.GetAllForElectionAsync(1)).ReturnsAsync(new List<ElectionVote>
        {
            new() { ElectionVoteId = 1, ElectionId = 1, CandidateId = 10, VoterId = 5, VotedAt = DateTime.UtcNow }
        });

        // Act
        var result = await _service.GetResultsAsync(1);

        // Assert
        result.VoterDetails.Should().ContainSingle(v => v.VoterId == 5 && v.CandidateIds.Contains(10));
    }

    [Fact]
    public async Task GetEligibleAsync_WhenEmployeeNotInEligibleSet_ShouldExcludeElection()
    {
        // Arrange
        var election = OpenElection();
        _electionRepositoryMock.Setup(r => r.GetByStatusAsync("Open")).ReturnsAsync(new List<Election> { election });
        _eligibilityServiceMock
            .Setup(s => s.GetEligibleEmployeeIdsAsync(election, It.IsAny<List<ElectionAudienceTarget>>()))
            .ReturnsAsync(new HashSet<long> { 999 });

        // Act
        var result = await _service.GetEligibleAsync(5);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetEligibleAsync_WhenEmployeeEligibleAndHasVoted_ShouldMarkHasVotedTrue()
    {
        // Arrange
        var election = OpenElection();
        _electionRepositoryMock.Setup(r => r.GetByStatusAsync("Open")).ReturnsAsync(new List<Election> { election });
        _voteRepositoryMock.Setup(r => r.HasVotedAsync(election.ElectionId, 5)).ReturnsAsync(true);

        // Act
        var result = await _service.GetEligibleAsync(5);

        // Assert
        result.Should().ContainSingle(e => e.ElectionId == election.ElectionId && e.HasVoted);
    }
}
