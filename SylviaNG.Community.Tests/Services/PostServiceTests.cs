using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMentionService> _mentionServiceMock;
    private readonly PostService _service;

    public PostServiceTests()
    {
        _postRepositoryMock = new Mock<IPostRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mentionServiceMock = new Mock<IMentionService>();
        _service = new PostService(_postRepositoryMock.Object, _employeeRepositoryMock.Object, _unitOfWorkMock.Object, _mentionServiceMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new PostCreateRequest { EmployeeId = 1, Type = "Update", Visibility = VisibilityEnum.Everyone, Content = "Hello" };
        _postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Post>()))
            .Callback<Post>(p => p.PostId = 10);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(10);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldFanOutMentions()
    {
        // Arrange
        var request = new PostCreateRequest
        {
            EmployeeId = 1,
            Type = "Update",
            Visibility = VisibilityEnum.Everyone,
            Content = "Hello @Bob",
            MentionedEmployeeIds = new List<long> { 2, 3 },
        };
        _postRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Post>()))
            .Callback<Post>(p => p.PostId = 10);

        // Act
        await _service.CreateAsync(request);

        // Assert
        _mentionServiceMock.Verify(m => m.CreateMentionsAsync("Post", 10, 1, request.MentionedEmployeeIds), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        // Act
        var act = () => _service.UpdateAsync(1, new PostUpdateRequest(), callerEmployeeId: 1, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_WhenCallerIsNotAuthorAndNotHrOrAdmin_ShouldThrowForbiddenException()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 1, IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        var act = () => _service.UpdateAsync(1, new PostUpdateRequest { Content = "Edited" }, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task UpdateAsync_WhenPostIsLocked_ShouldThrowForbiddenException()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 1, IsLocked = true };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        var act = () => _service.UpdateAsync(1, new PostUpdateRequest { Content = "Edited" }, callerEmployeeId: 1, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_ShouldApplyChangesAndSave()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 1, Content = "Old", IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        await _service.UpdateAsync(1, new PostUpdateRequest { Content = "New" }, callerEmployeeId: 1, isHrOrAdmin: false);

        // Assert
        post.Content.Should().Be("New");
        _postRepositoryMock.Verify(r => r.Update(post), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFanOutMentions()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 1, Content = "Old", IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        var request = new PostUpdateRequest { Content = "New @Bob", MentionedEmployeeIds = new List<long> { 5 } };

        // Act
        await _service.UpdateAsync(1, request, callerEmployeeId: 1, isHrOrAdmin: false);

        // Assert
        _mentionServiceMock.Verify(m => m.CreateMentionsAsync("Post", 1, 1, request.MentionedEmployeeIds), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCallerIsHrOrAdmin_ShouldBypassOwnershipCheck()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 1, Content = "Old", IsLocked = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        await _service.UpdateAsync(1, new PostUpdateRequest { Content = "New" }, callerEmployeeId: 2, isHrOrAdmin: true);

        // Assert
        post.Content.Should().Be("New");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        // Act
        var act = () => _service.DeleteAsync(1, callerEmployeeId: 1, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsNotAuthorAndNotHrOrAdmin_ShouldThrowForbiddenException()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 1 };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        var act = () => _service.DeleteAsync(1, callerEmployeeId: 2, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _postRepositoryMock.Verify(r => r.Delete(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsAuthor_ShouldDelete()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 1 };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        await _service.DeleteAsync(1, callerEmployeeId: 1, isHrOrAdmin: false);

        // Assert
        _postRepositoryMock.Verify(r => r.Delete(post), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenCallerIsHrOrAdmin_ShouldBypassOwnershipCheck()
    {
        // Arrange
        var post = new Post { PostId = 1, EmployeeId = 1 };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        // Act
        await _service.DeleteAsync(1, callerEmployeeId: 2, isHrOrAdmin: true);

        // Assert
        _postRepositoryMock.Verify(r => r.Delete(post), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
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
            Data = new List<Post> { new() { PostId = 1, Type = "Update", Visibility = VisibilityEnum.Everyone } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _postRepositoryMock.Setup(r => r.GetFeedPaginatedAsync(It.IsAny<PostFilterRequest>(), It.IsAny<long?>(), It.IsAny<long?>())).ReturnsAsync(pagedResult);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { EmployeeId = 1, DepartmentId = 5, SiteId = 9 });

        // Act
        var result = await _service.GetFeedPaginatedAsync(new PostFilterRequest(), callerEmployeeId: 1);

        // Assert
        result.Data.Should().ContainSingle(p => p.PostId == 1);
        result.TotalCount.Should().Be(1);
        _postRepositoryMock.Verify(r => r.GetFeedPaginatedAsync(It.IsAny<PostFilterRequest>(), 5, 9), Times.Once);
    }

    [Fact]
    public async Task GetFeedPaginatedAsync_WithEmployeeIdFilter_ShouldForwardItToRepository()
    {
        // Arrange
        var pagedResult = new PagedResult<Post>
        {
            Data = new List<Post> { new() { PostId = 1, EmployeeId = 7, Type = "Update", Visibility = VisibilityEnum.Everyone } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _postRepositoryMock.Setup(r => r.GetFeedPaginatedAsync(It.IsAny<PostFilterRequest>(), It.IsAny<long?>(), It.IsAny<long?>())).ReturnsAsync(pagedResult);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { EmployeeId = 1, DepartmentId = 5, SiteId = 9 });

        // Act
        await _service.GetFeedPaginatedAsync(new PostFilterRequest { EmployeeId = 7 }, callerEmployeeId: 1);

        // Assert
        _postRepositoryMock.Verify(r => r.GetFeedPaginatedAsync(It.Is<PostFilterRequest>(req => req.EmployeeId == 7), 5, 9), Times.Once);
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
