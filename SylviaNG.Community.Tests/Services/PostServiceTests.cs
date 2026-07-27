using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PostService _service;

    public PostServiceTests()
    {
        _postRepositoryMock = new Mock<IPostRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new PostService(_postRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new PostCreateRequest { EmployeeId = 1, Type = "Update", Visibility = "Public", Content = "Hello" };
        _postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Post>()))
            .Callback<Post>(p => p.PostId = 10);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(10);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        // Act
        var act = () => _service.UpdateAsync(1, new PostUpdateRequest());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_WhenPostIsLocked_ShouldThrowForbiddenException()
    {
        // Arrange
        var post = new Post { PostId = 1, IsLocked = true };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        var act = () => _service.UpdateAsync(1, new PostUpdateRequest { Content = "Edited" });

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_ShouldApplyChangesAndSave()
    {
        // Arrange
        var post = new Post { PostId = 1, Content = "Old", IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        await _service.UpdateAsync(1, new PostUpdateRequest { Content = "New" });

        // Assert
        post.Content.Should().Be("New");
        _postRepositoryMock.Verify(r => r.Update(post), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        // Act
        var act = () => _service.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        // Act
        var act = () => _service.GetByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetFeedPaginatedAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var pagedResult = new PagedResult<Post>
        {
            Data = new List<Post> { new() { PostId = 1, Type = "Update", Visibility = "Public" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _postRepositoryMock.Setup(r => r.GetFeedPaginatedAsync(It.IsAny<PagedRequest>())).ReturnsAsync(pagedResult);

        // Act
        var result = await _service.GetFeedPaginatedAsync(new PagedRequest());

        // Assert
        result.Data.Should().ContainSingle(p => p.PostId == 1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task SetLockedAsync_ShouldUpdateFlagAndSave()
    {
        // Arrange
        var post = new Post { PostId = 1, IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        await _service.SetLockedAsync(1, true);

        // Assert
        post.IsLocked.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetHiddenAsync_ShouldUpdateFlagAndSave()
    {
        // Arrange
        var post = new Post { PostId = 1, IsHidden = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        await _service.SetHiddenAsync(1, true);

        // Assert
        post.IsHidden.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
