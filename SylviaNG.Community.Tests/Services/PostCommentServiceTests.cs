using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.PostComments.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
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
    private readonly PostCommentService _service;

    public PostCommentServiceTests()
    {
        _commentRepositoryMock = new Mock<IPostCommentRepository>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new PostCommentService(_commentRepositoryMock.Object, _postRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var post = new Post { PostId = 1, IsLocked = false };
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
    public async Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PostComment?)null);

        // Act
        var act = () => _service.DeleteAsync(1, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCommentBelongsToDifferentPost_ShouldThrowNotFoundException()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new PostComment { CommentId = 5, PostId = 2 });

        // Act
        var act = () => _service.DeleteAsync(1, 5);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidRequest_ShouldDeleteAndSave()
    {
        // Arrange
        var comment = new PostComment { CommentId = 5, PostId = 1 };
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(comment);

        // Act
        await _service.DeleteAsync(1, 5);

        // Assert
        _commentRepositoryMock.Verify(r => r.Delete(comment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
