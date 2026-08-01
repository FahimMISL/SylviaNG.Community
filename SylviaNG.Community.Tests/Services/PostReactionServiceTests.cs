using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.PostReactions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class PostReactionServiceTests
{
    private readonly Mock<IPostReactionRepository> _reactionRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly PostReactionService _service;

    public PostReactionServiceTests()
    {
        _reactionRepositoryMock = new Mock<IPostReactionRepository>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _notificationServiceMock = new Mock<INotificationService>();
        _notificationServiceMock.Setup(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>())).ReturnsAsync(1L);
        _service = new PostReactionService(_reactionRepositoryMock.Object, _postRepositoryMock.Object, _unitOfWorkMock.Object, _notificationServiceMock.Object);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenNoExistingReaction_ShouldCreateNew()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1, EmployeeId = 9 });
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync((PostReaction?)null);
        _reactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PostReaction>()))
            .Callback<PostReaction>(x => x.ReactionId = 7);

        // Act
        var result = await _service.AddOrToggleAsync(1, new PostReactionAddRequest { EmployeeId = 2, ReactionType = ReactionTypeEnum.Like });

        // Assert
        result.Should().NotBeNull();
        result!.ReactionId.Should().Be(7);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenSameReactionExists_ShouldToggleOff()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1 });
        var existing = new PostReaction { ReactionId = 5, PostId = 1, EmployeeId = 2, ReactionType = ReactionTypeEnum.Like };
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync(existing);

        // Act
        var result = await _service.AddOrToggleAsync(1, new PostReactionAddRequest { EmployeeId = 2, ReactionType = ReactionTypeEnum.Like });

        // Assert
        result.Should().BeNull();
        _reactionRepositoryMock.Verify(r => r.Delete(existing), Times.Once);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenReactorIsNotPostAuthor_ShouldNotifyPostAuthor()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1, EmployeeId = 9 });
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync((PostReaction?)null);

        // Act
        await _service.AddOrToggleAsync(1, new PostReactionAddRequest { EmployeeId = 2, ReactionType = ReactionTypeEnum.Like });

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(
            r => r.EmployeeId == 9 && r.Category == "PostReaction" && r.RelatedEntityId == 1)), Times.Once);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenReactorIsPostAuthor_ShouldNotNotify()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1, EmployeeId = 2 });
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync((PostReaction?)null);

        // Act
        await _service.AddOrToggleAsync(1, new PostReactionAddRequest { EmployeeId = 2, ReactionType = ReactionTypeEnum.Like });

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Never);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenTogglingOff_ShouldNotNotify()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1, EmployeeId = 9 });
        var existing = new PostReaction { ReactionId = 5, PostId = 1, EmployeeId = 2, ReactionType = ReactionTypeEnum.Like };
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync(existing);

        // Act
        await _service.AddOrToggleAsync(1, new PostReactionAddRequest { EmployeeId = 2, ReactionType = ReactionTypeEnum.Like });

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Never);
    }
}
