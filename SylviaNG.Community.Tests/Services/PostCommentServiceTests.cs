using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.PostComments.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class PostCommentServiceTests
{
    private readonly Mock<IPostCommentRepository> _commentRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IMentionService> _mentionServiceMock;
    private readonly PostCommentService _service;

    public PostCommentServiceTests()
    {
        _commentRepositoryMock = new Mock<IPostCommentRepository>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _notificationServiceMock = new Mock<INotificationService>();
        _notificationServiceMock.Setup(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>())).ReturnsAsync(1L);
        _mentionServiceMock = new Mock<IMentionService>();
        _service = new PostCommentService(_commentRepositoryMock.Object, _postRepositoryMock.Object, _unitOfWorkMock.Object, _notificationServiceMock.Object, _mentionServiceMock.Object);
    }

    [Fact]
    public async Task AddAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 2, IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        _commentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PostComment>()))
            .Callback<PostComment>(c => c.CommentId = 5);

        var request = new PostCommentAddRequest { EmployeeId = 2, Content = "Nice post!" };

        // Act
        var result = await _service.AddAsync(1, request);

        // Assert
        result.Should().Be(5);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldFanOutMentions()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 2, IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        _commentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PostComment>()))
            .Callback<PostComment>(c => c.CommentId = 5);

        var request = new PostCommentAddRequest { EmployeeId = 2, Content = "Nice @Bob", MentionedEmployeeIds = new List<long> { 8 } };

        // Act
        await _service.AddAsync(1, request);

        // Assert
        _mentionServiceMock.Verify(m => m.CreateMentionsAsync("PostComment", 5, 2, request.MentionedEmployeeIds), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenPostNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        // Act
        var act = () => _service.AddAsync(1, new PostCommentAddRequest { EmployeeId = 2, Content = "Hi" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddAsync_WhenPostIsLocked_ShouldThrowForbiddenException()
    {
        // Arrange
        var post = new Post { PostId = 1, IsLocked = true };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        var act = () => _service.AddAsync(1, new PostCommentAddRequest { EmployeeId = 2, Content = "Hi" });

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task AddAsync_WithParentCommentFromDifferentPost_ShouldThrowNotFoundException()
    {
        // Arrange
        var post = new Post { PostId = 1, IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(new PostComment { CommentId = 99, PostId = 2 });

        var request = new PostCommentAddRequest { EmployeeId = 2, Content = "Reply", ParentCommentId = 99 };

        // Act
        var act = () => _service.AddAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddAsync_WithValidParentComment_ShouldAddReply()
    {
        // Arrange
        var post = new Post { PostId = 1, IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(new PostComment { CommentId = 99, PostId = 1 });
        _commentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PostComment>()))
            .Callback<PostComment>(c => c.CommentId = 6);

        var request = new PostCommentAddRequest { EmployeeId = 2, Content = "Reply", ParentCommentId = 99 };

        // Act
        var result = await _service.AddAsync(1, request);

        // Assert
        result.Should().Be(6);
    }

    [Fact]
    public async Task AddAsync_WhenCommenterIsNotPostAuthor_ShouldNotifyPostAuthor()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 9, IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        var request = new PostCommentAddRequest { EmployeeId = 2, Content = "Nice post!" };

        // Act
        await _service.AddAsync(1, request);

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(
            r => r.EmployeeId == 9 && r.Category == "PostComment" && r.RelatedEntityId == 1)), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenCommenterIsPostAuthor_ShouldNotNotify()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 2, IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        var request = new PostCommentAddRequest { EmployeeId = 2, Content = "My own thoughts" };

        // Act
        await _service.AddAsync(1, request);

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_WithReply_ShouldNotifyBothPostAuthorAndParentCommenter()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 9, IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(new PostComment { CommentId = 99, PostId = 1, EmployeeId = 7 });

        var request = new PostCommentAddRequest { EmployeeId = 2, Content = "Reply", ParentCommentId = 99 };

        // Act
        await _service.AddAsync(1, request);

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(
            r => r.EmployeeId == 9 && r.Category == "PostComment")), Times.Once);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(
            r => r.EmployeeId == 7 && r.Category == "CommentReply" && r.RelatedEntityId == 99)), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithReply_WhenParentCommenterIsPostAuthor_ShouldNotifyOnce()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 9, IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(new PostComment { CommentId = 99, PostId = 1, EmployeeId = 9 });

        var request = new PostCommentAddRequest { EmployeeId = 2, Content = "Reply", ParentCommentId = 99 };

        // Act
        await _service.AddAsync(1, request);

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Once);
    }

    [Fact]
    public async Task GetByPostIdAsync_ShouldReturnMappedResults()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByPostIdAsync(1))
            .ReturnsAsync(new List<PostComment> { new() { CommentId = 1, PostId = 1, Content = "Hi" } });

        // Act
        var result = await _service.GetByPostIdAsync(1);

        // Assert
        result.Should().ContainSingle(c => c.CommentId == 1);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PostComment?)null);

        // Act
        var act = () => _service.UpdateAsync(1, 1, new PostCommentUpdateRequest { Content = "Edited" }, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_WhenCallerIsNotAuthorAndNotHrOrAdmin_ShouldThrowForbiddenException()
    {
        // Arrange
        var comment = new PostComment { CommentId = 5, PostId = 1, EmployeeId = 2 };
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(comment);

        // Act
        var act = () => _service.UpdateAsync(1, 5, new PostCommentUpdateRequest { Content = "Edited" }, callerEmployeeId: 3, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _commentRepositoryMock.Verify(r => r.Update(It.IsAny<PostComment>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenPostIsLocked_ShouldThrowForbiddenException()
    {
        // Arrange
        var comment = new PostComment { CommentId = 5, PostId = 1, EmployeeId = 2 };
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(comment);
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1, IsLocked = true });

        // Act
        var act = () => _service.UpdateAsync(1, 5, new PostCommentUpdateRequest { Content = "Edited" }, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_ShouldApplyChangesAndSave()
    {
        // Arrange
        var comment = new PostComment { CommentId = 5, PostId = 1, EmployeeId = 2, Content = "Old" };
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(comment);
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1, IsLocked = false });

        // Act
        await _service.UpdateAsync(1, 5, new PostCommentUpdateRequest { Content = "New" }, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        comment.Content.Should().Be("New");
        _commentRepositoryMock.Verify(r => r.Update(comment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCallerIsHrOrAdmin_ShouldBypassOwnershipCheck()
    {
        // Arrange
        var comment = new PostComment { CommentId = 5, PostId = 1, EmployeeId = 2, Content = "Old" };
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(comment);
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1, IsLocked = false });

        // Act
        await _service.UpdateAsync(1, 5, new PostCommentUpdateRequest { Content = "New" }, callerEmployeeId: 999, isHrOrAdmin: true);

        // Assert
        comment.Content.Should().Be("New");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PostComment?)null);

        // Act
        var act = () => _service.DeleteAsync(1, 1, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCommentBelongsToDifferentPost_ShouldThrowNotFoundException()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new PostComment { CommentId = 5, PostId = 2, EmployeeId = 2 });

        // Act
        var act = () => _service.DeleteAsync(1, 5, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsNotAuthorAndNotHrOrAdmin_ShouldThrowForbiddenException()
    {
        // Arrange
        var comment = new PostComment { CommentId = 5, PostId = 1, EmployeeId = 2 };
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(comment);

        // Act
        var act = () => _service.DeleteAsync(1, 5, callerEmployeeId: 3, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _commentRepositoryMock.Verify(r => r.Delete(It.IsAny<PostComment>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidRequest_ShouldDeleteAndSave()
    {
        // Arrange
        var comment = new PostComment { CommentId = 5, PostId = 1, EmployeeId = 2 };
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(comment);

        // Act
        await _service.DeleteAsync(1, 5, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        _commentRepositoryMock.Verify(r => r.Delete(comment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsHrOrAdmin_ShouldBypassOwnershipCheck()
    {
        // Arrange
        var comment = new PostComment { CommentId = 5, PostId = 1, EmployeeId = 2 };
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(comment);

        // Act
        await _service.DeleteAsync(1, 5, callerEmployeeId: 999, isHrOrAdmin: true);

        // Assert
        _commentRepositoryMock.Verify(r => r.Delete(comment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
