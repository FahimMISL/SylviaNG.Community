using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<ITaskCommentRepository> _taskCommentRepositoryMock;
    private readonly Mock<ITaskAttachmentRepository> _taskAttachmentRepositoryMock;
    private readonly Mock<ITaskHistoryRepository> _taskHistoryRepositoryMock;
    private readonly Mock<ITaskTagRepository> _taskTagRepositoryMock;
    private readonly Mock<ITaskTagMappingRepository> _taskTagMappingRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _taskCommentRepositoryMock = new Mock<ITaskCommentRepository>();
        _taskAttachmentRepositoryMock = new Mock<ITaskAttachmentRepository>();
        _taskHistoryRepositoryMock = new Mock<ITaskHistoryRepository>();
        _taskTagRepositoryMock = new Mock<ITaskTagRepository>();
        _taskTagMappingRepositoryMock = new Mock<ITaskTagMappingRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new TaskService(
            _taskRepositoryMock.Object,
            _taskCommentRepositoryMock.Object,
            _taskAttachmentRepositoryMock.Object,
            _taskHistoryRepositoryMock.Object,
            _taskTagRepositoryMock.Object,
            _taskTagMappingRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithValidRequest_ShouldReturnTaskId()
    {
        // Arrange
        var request = new TaskCreateRequest
        {
            TeamId = 1,
            AssignedBy = 2,
            AssignedTo = 3,
            Title = "Prepare report",
            Priority = "High",
            Status = "Open"
        };

        _taskRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskEntity>()))
            .Callback<TaskEntity>(t => t.TaskId = 10);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(10);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TaskEntity?)null);

        // Act
        var act = () => _service.UpdateAsync(1, new TaskUpdateRequest(), 99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WhenStatusChanges_ShouldInsertTaskHistoryRow()
    {
        // Arrange
        var entity = new TaskEntity
        {
            TaskId = 1,
            TeamId = 1,
            AssignedBy = 2,
            AssignedTo = 3,
            Title = "Prepare report",
            Priority = "High",
            TaskStatus = "Open"
        };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var request = new TaskUpdateRequest { Status = "Completed" };

        // Act
        await _service.UpdateAsync(1, request, 99);

        // Assert
        entity.TaskStatus.Should().Be("Completed");
        _taskHistoryRepositoryMock.Verify(r => r.AddAsync(It.Is<TaskHistory>(h =>
            h.TaskId == 1 &&
            h.Action == "StatusChanged" &&
            h.OldValue == "Open" &&
            h.NewValue == "Completed" &&
            h.ChangedBy == 99)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WhenChangedByNotProvided_ShouldFallBackToAssignedBy()
    {
        // Arrange
        var entity = new TaskEntity
        {
            TaskId = 1,
            TeamId = 1,
            AssignedBy = 2,
            AssignedTo = 3,
            Title = "Prepare report",
            Priority = "High",
            TaskStatus = "Open"
        };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var request = new TaskUpdateRequest { Priority = "Low" };

        // Act
        await _service.UpdateAsync(1, request, null);

        // Assert
        _taskHistoryRepositoryMock.Verify(r => r.AddAsync(It.Is<TaskHistory>(h =>
            h.Action == "PriorityChanged" && h.ChangedBy == 2)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WhenNoTrackedFieldChanges_ShouldNotInsertTaskHistory()
    {
        // Arrange
        var entity = new TaskEntity
        {
            TaskId = 1,
            TeamId = 1,
            AssignedBy = 2,
            AssignedTo = 3,
            Title = "Prepare report",
            Priority = "High",
            TaskStatus = "Open"
        };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var request = new TaskUpdateRequest { Title = "Prepare final report" };

        // Act
        await _service.UpdateAsync(1, request, 99);

        // Assert
        _taskHistoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskHistory>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TaskEntity?)null);

        // Act
        var act = () => _service.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TaskEntity?)null);

        // Act
        var act = () => _service.GetByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task AddCommentAsync_WhenTaskNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TaskEntity?)null);

        // Act
        var act = () => _service.AddCommentAsync(1, new TaskCommentAddRequest { EmployeeId = 5, Comment = "Looks good" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task AddCommentAsync_WithValidRequest_ShouldReturnCommentId()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new TaskEntity { TaskId = 1 });
        _taskCommentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskComment>()))
            .Callback<TaskComment>(c => c.CommentId = 7);

        // Act
        var result = await _service.AddCommentAsync(1, new TaskCommentAddRequest { EmployeeId = 5, Comment = "Looks good" });

        // Assert
        result.Should().Be(7);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task RemoveAttachmentAsync_WhenAttachmentBelongsToDifferentTask_ShouldThrowNotFoundException()
    {
        // Arrange
        _taskAttachmentRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new TaskAttachment { AttachmentId = 1, TaskId = 999 });

        // Act
        var act = () => _service.RemoveAttachmentAsync(1, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task AssignTagAsync_WhenAlreadyAssigned_ShouldThrowDuplicateException()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new TaskEntity { TaskId = 1 });
        _taskTagMappingRepositoryMock.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(true);

        // Act
        var act = () => _service.AssignTagAsync(1, new TaskTagAssignRequest { TagId = 2 });

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTagsAsync_ShouldReturnMappedTagResponses()
    {
        // Arrange
        _taskTagMappingRepositoryMock.Setup(r => r.GetByTaskIdAsync(1))
            .ReturnsAsync(new List<TaskTagMapping> { new() { MappingId = 1, TaskId = 1, TagId = 2 } });
        _taskTagRepositoryMock.Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(new TaskTag { TagId = 2, Name = "Urgent" });

        // Act
        var result = await _service.GetTagsAsync(1);

        // Assert
        result.Should().ContainSingle(t => t.TagId == 2 && t.Name == "Urgent");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetHistoryAsync_ShouldReturnMappedHistoryResponses()
    {
        // Arrange
        _taskHistoryRepositoryMock.Setup(r => r.GetByTaskIdAsync(1))
            .ReturnsAsync(new List<TaskHistory>
            {
                new() { HistoryId = 1, TaskId = 1, Action = "StatusChanged", OldValue = "Open", NewValue = "Completed", ChangedBy = 99 }
            });

        // Act
        var result = await _service.GetHistoryAsync(1);

        // Assert
        result.Should().ContainSingle(h => h.HistoryId == 1 && h.Action == "StatusChanged");
    }
}
