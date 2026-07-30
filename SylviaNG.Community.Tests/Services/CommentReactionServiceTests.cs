using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.CommentReactions.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class CommentReactionServiceTests
{
    private readonly Mock<ICommentReactionRepository> _reactionRepositoryMock;
    private readonly Mock<IPostCommentRepository> _commentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly CommentReactionService _service;

    public CommentReactionServiceTests()
    {
        _reactionRepositoryMock = new Mock<ICommentReactionRepository>();
        _commentRepositoryMock = new Mock<IPostCommentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _notificationServiceMock = new Mock<INotificationService>();
        _notificationServiceMock.Setup(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>())).ReturnsAsync(1L);
        _service = new CommentReactionService(_reactionRepositoryMock.Object, _commentRepositoryMock.Object, _unitOfWorkMock.Object, _notificationServiceMock.Object);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenCommentNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PostComment?)null);

        // Act
        var act = () => _service.AddOrToggleAsync(1, new CommentReactionAddRequest { EmployeeId = 2, ReactionType = ReactionTypeEnum.Like });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenNoExistingReaction_ShouldCreateNew()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PostComment { CommentId = 1, EmployeeId = 9 });
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync((CommentReaction?)null);
        _reactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CommentReaction>()))
            .Callback<CommentReaction>(x => x.ReactionId = 9);

        // Act
        var result = await _service.AddOrToggleAsync(1, new CommentReactionAddRequest { EmployeeId = 2, ReactionType = ReactionTypeEnum.Love });

        // Assert
        result.Should().NotBeNull();
        result!.ReactionId.Should().Be(9);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenReactorIsNotCommentAuthor_ShouldNotifyCommentAuthor()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PostComment { CommentId = 1, EmployeeId = 9 });
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync((CommentReaction?)null);

        // Act
        await _service.AddOrToggleAsync(1, new CommentReactionAddRequest { EmployeeId = 2, ReactionType = ReactionTypeEnum.Love });

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(
            r => r.EmployeeId == 9 && r.Category == "CommentReaction" && r.RelatedEntityId == 1)), Times.Once);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenReactorIsCommentAuthor_ShouldNotNotify()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PostComment { CommentId = 1, EmployeeId = 2 });
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync((CommentReaction?)null);

        // Act
        await _service.AddOrToggleAsync(1, new CommentReactionAddRequest { EmployeeId = 2, ReactionType = ReactionTypeEnum.Love });

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Never);
    }
}
