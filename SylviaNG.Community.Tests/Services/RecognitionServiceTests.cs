using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class RecognitionServiceTests
{
    private readonly Mock<IRecognitionRepository> _recognitionRepositoryMock;
    private readonly Mock<IRecognitionReactionRepository> _recognitionReactionRepositoryMock;
    private readonly Mock<IRecognitionCommentRepository> _recognitionCommentRepositoryMock;
    private readonly Mock<IBadgeRepository> _badgeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RecognitionService _service;

    public RecognitionServiceTests()
    {
        _recognitionRepositoryMock = new Mock<IRecognitionRepository>();
        _recognitionReactionRepositoryMock = new Mock<IRecognitionReactionRepository>();
        _recognitionCommentRepositoryMock = new Mock<IRecognitionCommentRepository>();
        _badgeRepositoryMock = new Mock<IBadgeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new RecognitionService(
            _recognitionRepositoryMock.Object,
            _recognitionReactionRepositoryMock.Object,
            _recognitionCommentRepositoryMock.Object,
            _badgeRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnIdAndUseCallerAsSender()
    {
        // Arrange
        var request = new RecognitionCreateRequest
        {
            RecipientId = 2,
            RecognitionType = "Peer",
            Message = "Great job!"
        };

        Recognition? added = null;
        _recognitionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Recognition>()))
            .Callback<Recognition>(r =>
            {
                r.RecognitionId = 1;
                added = r;
            });

        // Act
        var result = await _service.CreateAsync(request, callerEmployeeId: 1, isHrOrAdmin: false);

        // Assert
        result.Should().Be(1);
        added!.SenderId.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithZeroCallerEmployeeId_ShouldThrowForbiddenException()
    {
        // Arrange
        var request = new RecognitionCreateRequest { RecipientId = 2, RecognitionType = "Peer" };

        // Act
        var act = () => _service.CreateAsync(request, callerEmployeeId: 0, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CreateAsync_WithIsHrIssuedFromNonHrCaller_ShouldThrowForbiddenException()
    {
        // Arrange
        var request = new RecognitionCreateRequest { RecipientId = 2, RecognitionType = "Award", IsHrIssued = true };

        // Act
        var act = () => _service.CreateAsync(request, callerEmployeeId: 1, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CreateAsync_WithInvalidBadgeId_ShouldThrowNotFoundException()
    {
        // Arrange
        var request = new RecognitionCreateRequest { RecipientId = 2, RecognitionType = "Peer", BadgeId = 99 };
        _badgeRepositoryMock.Setup(b => b.GetByIdAsync(99)).ReturnsAsync((Badge?)null);

        // Act
        var act = () => _service.CreateAsync(request, callerEmployeeId: 1, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _recognitionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Recognition?)null);

        // Act
        var act = () => _service.GetByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddReactionAsync_WhenRecognitionNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _recognitionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Recognition?)null);

        // Act
        var act = () => _service.AddReactionAsync(1, new RecognitionReactionAddRequest { ReactionType = "Like" }, callerEmployeeId: 5);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddReactionAsync_WhenEmployeeAlreadyReacted_ShouldUpdateExistingReaction()
    {
        // Arrange
        _recognitionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Recognition { RecognitionId = 1 });
        var existing = new RecognitionReaction { ReactionId = 7, RecognitionId = 1, EmployeeId = 5, ReactionType = "Like" };
        _recognitionReactionRepositoryMock.Setup(r => r.GetAsync(1, 5)).ReturnsAsync(existing);

        // Act
        var result = await _service.AddReactionAsync(1, new RecognitionReactionAddRequest { ReactionType = "Clap" }, callerEmployeeId: 5);

        // Assert
        result.Should().Be(7);
        existing.ReactionType.Should().Be("Clap");
        _recognitionReactionRepositoryMock.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public async Task RemoveReactionAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _recognitionReactionRepositoryMock.Setup(r => r.GetAsync(1, 5)).ReturnsAsync((RecognitionReaction?)null);

        // Act
        var act = () => _service.RemoveReactionAsync(1, employeeId: 5, callerEmployeeId: 5, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RemoveReactionAsync_WhenCallerIsNotOwnerAndNotHr_ShouldThrowForbiddenException()
    {
        // Arrange
        var existing = new RecognitionReaction { ReactionId = 7, RecognitionId = 1, EmployeeId = 5, ReactionType = "Like" };
        _recognitionReactionRepositoryMock.Setup(r => r.GetAsync(1, 5)).ReturnsAsync(existing);

        // Act
        var act = () => _service.RemoveReactionAsync(1, employeeId: 5, callerEmployeeId: 9, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
