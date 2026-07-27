using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Tests.Services;

public class TaskTagServiceTests
{
    private readonly Mock<ITaskTagRepository> _taskTagRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TaskTagService _service;

    public TaskTagServiceTests()
    {
        _taskTagRepositoryMock = new Mock<ITaskTagRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new TaskTagService(_taskTagRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new TaskTagCreateRequest { Name = "Urgent" };
        _taskTagRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(false);
        _taskTagRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskTag>()))
            .Callback<TaskTag>(t => t.TagId = 1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        // Arrange
        var request = new TaskTagCreateRequest { Name = "Urgent" };
        _taskTagRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _taskTagRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TaskTag?)null);

        // Act
        var act = () => _service.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
