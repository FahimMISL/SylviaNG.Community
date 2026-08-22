using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<ITaskCommentRepository> _taskCommentRepositoryMock;
    private readonly Mock<ITaskAttachmentRepository> _taskAttachmentRepositoryMock;
    private readonly Mock<ITaskHistoryRepository> _taskHistoryRepositoryMock;
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<ITeamMemberRepository> _teamMemberRepositoryMock;
    private readonly Mock<IRecurringTaskRepository> _recurringTaskRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _taskCommentRepositoryMock = new Mock<ITaskCommentRepository>();
        _taskAttachmentRepositoryMock = new Mock<ITaskAttachmentRepository>();
        _taskHistoryRepositoryMock = new Mock<ITaskHistoryRepository>();
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _teamMemberRepositoryMock = new Mock<ITeamMemberRepository>();
        _recurringTaskRepositoryMock = new Mock<IRecurringTaskRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new TaskService(
            _taskRepositoryMock.Object,
            _taskCommentRepositoryMock.Object,
            _taskAttachmentRepositoryMock.Object,
            _taskHistoryRepositoryMock.Object,
            _teamRepositoryMock.Object,
            _teamMemberRepositoryMock.Object,
            _recurringTaskRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _notificationServiceMock.Object,
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
        var result = await _service.CreateAsync(request, callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        result.Should().Be(10);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r =>
            r.EmployeeId == 3 && r.Category == "Task" && r.RelatedEntityId == 10)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TaskEntity?)null);

        // Act
        var act = () => _service.UpdateAsync(1, new TaskUpdateRequest(), 99, isHrOrAdmin: true);

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
        await _service.UpdateAsync(1, request, 99, isHrOrAdmin: true);

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
        await _service.UpdateAsync(1, request, null, isHrOrAdmin: true);

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
        await _service.UpdateAsync(1, request, 99, isHrOrAdmin: true);

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
        var act = () => _service.DeleteAsync(1, callerEmployeeId: null, isHrOrAdmin: true);

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
        var act = () => _service.AddCommentAsync(1, new TaskCommentAddRequest { EmployeeId = 5, Comment = "Looks good" }, callerEmployeeId: 5, isHrOrAdmin: false);

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
        var result = await _service.AddCommentAsync(1, new TaskCommentAddRequest { EmployeeId = 5, Comment = "Looks good" }, callerEmployeeId: 5, isHrOrAdmin: true);

        // Assert
        result.Should().Be(7);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddCommentAsync_WhenAssigneeComments_ShouldNotifyTheAssigner()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new TaskEntity { TaskId = 1, AssignedBy = 9, AssignedTo = 5, Title = "Report" });

        // Act - the assignee (5) comments.
        await _service.AddCommentAsync(1, new TaskCommentAddRequest { EmployeeId = 5, Comment = "Done" }, callerEmployeeId: 5, isHrOrAdmin: false);

        // Assert - the assigner (9) is notified.
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r =>
            r.EmployeeId == 9 && r.Category == "Task")), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddCommentAsync_WhenAssignerComments_ShouldNotifyTheAssignee()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new TaskEntity { TaskId = 1, AssignedBy = 9, AssignedTo = 5, Title = "Report" });

        // Act - the assigner (9) comments.
        await _service.AddCommentAsync(1, new TaskCommentAddRequest { EmployeeId = 9, Comment = "Any update?" }, callerEmployeeId: 9, isHrOrAdmin: false);

        // Assert - the assignee (5) is notified.
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r =>
            r.EmployeeId == 5 && r.Category == "Task")), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddCommentAsync_WhenTaskIsSelfAssigned_ShouldNotNotifyTheCommenter()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new TaskEntity { TaskId = 1, AssignedBy = 5, AssignedTo = 5, Title = "Report" });

        // Act
        await _service.AddCommentAsync(1, new TaskCommentAddRequest { EmployeeId = 5, Comment = "Note to self" }, callerEmployeeId: 5, isHrOrAdmin: false);

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task RemoveAttachmentAsync_WhenAttachmentBelongsToDifferentTask_ShouldThrowNotFoundException()
    {
        // Arrange
        _taskAttachmentRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new TaskAttachment { AttachmentId = 1, TaskId = 999 });

        // Act
        var act = () => _service.RemoveAttachmentAsync(1, 1, callerEmployeeId: 1, isHrOrAdmin: true);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
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

    [Fact]
    public async System.Threading.Tasks.Task GetMyPaginatedAsync_ShouldForceAssignedToTheCallerRegardlessOfRequest()
    {
        // Arrange - caller supplies an unrelated AssignedTo/AssignedBy, which must be overridden.
        var request = new TaskFilterRequest { AssignedTo = 999, AssignedBy = 888 };
        _taskRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<TaskFilterRequest>()))
            .ReturnsAsync(new PagedResult<TaskEntity> { Data = new List<TaskEntity>(), TotalCount = 0, PageNumber = 1, PageSize = 10 });

        // Act
        await _service.GetMyPaginatedAsync(request, callerEmployeeId: 42);

        // Assert
        _taskRepositoryMock.Verify(r => r.GetPaginatedAsync(It.Is<TaskFilterRequest>(f =>
            f.AssignedTo == 42 && f.AssignedBy == null && !f.IndividualOnly)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetMyPaginatedAsync_WhenCallerHasNoEmployeeId_ShouldForceImpossibleId()
    {
        // Arrange
        var request = new TaskFilterRequest();
        _taskRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<TaskFilterRequest>()))
            .ReturnsAsync(new PagedResult<TaskEntity> { Data = new List<TaskEntity>(), TotalCount = 0, PageNumber = 1, PageSize = 10 });

        // Act
        await _service.GetMyPaginatedAsync(request, callerEmployeeId: null);

        // Assert
        _taskRepositoryMock.Verify(r => r.GetPaginatedAsync(It.Is<TaskFilterRequest>(f => f.AssignedTo == -1)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAssignedByMePaginatedAsync_ShouldForceAssignedByAndIndividualOnly()
    {
        // Arrange
        var request = new TaskFilterRequest { AssignedTo = 999, TeamId = 5 };
        _taskRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<TaskFilterRequest>()))
            .ReturnsAsync(new PagedResult<TaskEntity> { Data = new List<TaskEntity>(), TotalCount = 0, PageNumber = 1, PageSize = 10 });

        // Act
        await _service.GetAssignedByMePaginatedAsync(request, callerEmployeeId: 7);

        // Assert
        _taskRepositoryMock.Verify(r => r.GetPaginatedAsync(It.Is<TaskFilterRequest>(f =>
            f.AssignedBy == 7 && f.AssignedTo == null && f.IndividualOnly)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTeamPaginatedAsync_WhenCallerIsNeitherSupervisorNorHrAdmin_ShouldThrowForbiddenException()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, SupervisorId = 55 });
        _teamMemberRepositoryMock.Setup(r => r.ExistsAsync(1, 7)).ReturnsAsync(false);

        // Act
        var act = () => _service.GetTeamPaginatedAsync(1, new TaskFilterRequest(), callerEmployeeId: 7, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTeamPaginatedAsync_WhenCallerIsThatTeamsSupervisor_ShouldForceTeamIdAndSucceed()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, SupervisorId = 7 });
        _taskRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<TaskFilterRequest>()))
            .ReturnsAsync(new PagedResult<TaskEntity> { Data = new List<TaskEntity>(), TotalCount = 0, PageNumber = 1, PageSize = 10 });

        // Act
        await _service.GetTeamPaginatedAsync(1, new TaskFilterRequest(), callerEmployeeId: 7, isHrOrAdmin: false);

        // Assert
        _taskRepositoryMock.Verify(r => r.GetPaginatedAsync(It.Is<TaskFilterRequest>(f => f.TeamId == 1)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTeamPaginatedAsync_WhenCallerIsAnActiveTeamMemberButNotSupervisor_ShouldSucceed()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Team { TeamId = 1, SupervisorId = 55 });
        _teamMemberRepositoryMock.Setup(r => r.ExistsAsync(1, 7)).ReturnsAsync(true);
        _taskRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<TaskFilterRequest>()))
            .ReturnsAsync(new PagedResult<TaskEntity> { Data = new List<TaskEntity>(), TotalCount = 0, PageNumber = 1, PageSize = 10 });

        // Act
        await _service.GetTeamPaginatedAsync(1, new TaskFilterRequest(), callerEmployeeId: 7, isHrOrAdmin: false);

        // Assert
        _taskRepositoryMock.Verify(r => r.GetPaginatedAsync(It.Is<TaskFilterRequest>(f => f.TeamId == 1)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTeamPaginatedAsync_WhenTeamNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _teamRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Team?)null);

        // Act
        var act = () => _service.GetTeamPaginatedAsync(1, new TaskFilterRequest(), callerEmployeeId: 7, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task BulkReassignAsync_ShouldReassignEveryTaskAndRecordHistory()
    {
        // Arrange
        var task1 = new TaskEntity { TaskId = 1, AssignedBy = 9, AssignedTo = 2 };
        var task2 = new TaskEntity { TaskId = 2, AssignedBy = 9, AssignedTo = 3 };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task1);
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(task2);

        // Act - caller (9) is the assigner of both tasks.
        await _service.BulkReassignAsync(new List<long> { 1, 2 }, newAssignedTo: 5, callerEmployeeId: 9, isHrOrAdmin: false);

        // Assert
        task1.AssignedTo.Should().Be(5);
        task2.AssignedTo.Should().Be(5);
        _taskHistoryRepositoryMock.Verify(r => r.AddAsync(It.Is<TaskHistory>(h =>
            h.TaskId == 1 && h.Action == "AssignedToChanged" && h.NewValue == "5")), Times.Once);
        _taskHistoryRepositoryMock.Verify(r => r.AddAsync(It.Is<TaskHistory>(h =>
            h.TaskId == 2 && h.Action == "AssignedToChanged" && h.NewValue == "5")), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Never);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r =>
            r.EmployeeId == 5 && r.Category == "Task" && r.RelatedEntityId == 1)), Times.Once);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r =>
            r.EmployeeId == 5 && r.Category == "Task" && r.RelatedEntityId == 2)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task BulkReassignAsync_WhenCallerLacksAccessToOneTask_ShouldRollbackAndThrowForbidden()
    {
        // Arrange - caller (9) is the assigner of task 1 but has no relation to task 2.
        var task1 = new TaskEntity { TaskId = 1, AssignedBy = 9, AssignedTo = 2 };
        var task2 = new TaskEntity { TaskId = 2, AssignedBy = 55, AssignedTo = 3 };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task1);
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(task2);

        // Act
        var act = () => _service.BulkReassignAsync(new List<long> { 1, 2 }, newAssignedTo: 5, callerEmployeeId: 9, isHrOrAdmin: false);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task BulkCancelAsync_ShouldDeleteEveryTask()
    {
        // Arrange
        var task1 = new TaskEntity { TaskId = 1, AssignedBy = 9, AssignedTo = 2 };
        var task2 = new TaskEntity { TaskId = 2, AssignedBy = 9, AssignedTo = 3 };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task1);
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(task2);

        // Act
        await _service.BulkCancelAsync(new List<long> { 1, 2 }, callerEmployeeId: 9, isHrOrAdmin: false);

        // Assert
        _taskRepositoryMock.Verify(r => r.Delete(task1), Times.Once);
        _taskRepositoryMock.Verify(r => r.Delete(task2), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WhenTaskJustCompletedWithActiveRecurringSeries_ShouldGenerateNextInstance()
    {
        // Arrange
        var dueDate = new DateTime(2026, 1, 1);
        var entity = new TaskEntity
        {
            TaskId = 1, AssignedBy = 9, AssignedTo = 2, Title = "Weekly report", Priority = "High",
            TaskStatus = "InProgress", DueDate = dueDate, RecurringTaskId = 5, ReminderDays = 2
        };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _recurringTaskRepositoryMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new RecurringTask { RecurringTaskId = 5, Frequency = "Weekly", IntervalValue = 1, IsActive = true });

        // Act - caller (9) is the assigner.
        await _service.UpdateAsync(1, new TaskUpdateRequest { Status = "Completed" }, changedBy: 9, isHrOrAdmin: false);

        // Assert
        _taskRepositoryMock.Verify(r => r.AddAsync(It.Is<TaskEntity>(t =>
            t.AssignedTo == 2 && t.RecurringTaskId == 5 && t.TaskStatus == "Assigned" && t.DueDate == dueDate.AddDays(7))), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WhenRecurringSeriesIsInactive_ShouldNotGenerateNextInstance()
    {
        // Arrange
        var entity = new TaskEntity
        {
            TaskId = 1, AssignedBy = 9, AssignedTo = 2, Title = "T", Priority = "High",
            TaskStatus = "InProgress", DueDate = DateTime.UtcNow, RecurringTaskId = 5
        };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _recurringTaskRepositoryMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new RecurringTask { RecurringTaskId = 5, Frequency = "Daily", IntervalValue = 1, IsActive = false });

        // Act
        await _service.UpdateAsync(1, new TaskUpdateRequest { Status = "Completed" }, changedBy: 9, isHrOrAdmin: false);

        // Assert
        _taskRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskEntity>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WhenNextOccurrenceIsPastSeriesEndDate_ShouldNotGenerateNextInstance()
    {
        // Arrange
        var dueDate = new DateTime(2026, 1, 1);
        var entity = new TaskEntity
        {
            TaskId = 1, AssignedBy = 9, AssignedTo = 2, Title = "T", Priority = "High",
            TaskStatus = "InProgress", DueDate = dueDate, RecurringTaskId = 5
        };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _recurringTaskRepositoryMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new RecurringTask { RecurringTaskId = 5, Frequency = "Weekly", IntervalValue = 1, IsActive = true, EndDate = dueDate.AddDays(3) });

        // Act
        await _service.UpdateAsync(1, new TaskUpdateRequest { Status = "Completed" }, changedBy: 9, isHrOrAdmin: false);

        // Assert
        _taskRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskEntity>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WhenTaskWasAlreadyCompleted_ShouldNotGenerateAgain()
    {
        // Arrange - re-saving an already-completed task (e.g. editing its title) must not
        // re-trigger generation.
        var entity = new TaskEntity
        {
            TaskId = 1, AssignedBy = 9, AssignedTo = 2, Title = "T", Priority = "High",
            TaskStatus = "Completed", DueDate = DateTime.UtcNow, RecurringTaskId = 5
        };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.UpdateAsync(1, new TaskUpdateRequest { Status = "Completed" }, changedBy: 9, isHrOrAdmin: false);

        // Assert
        _taskRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskEntity>()), Times.Never);
        _recurringTaskRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task GenerateReportAsync_WhenTaskNotCompleted_ShouldThrowValidationException()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new TaskEntity { TaskId = 1, TaskStatus = "InProgress" });

        // Act
        var act = () => _service.GenerateReportAsync(1);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GenerateReportAsync_WhenTaskCompleted_ShouldReturnTextReportWithCommentsAndHistory()
    {
        // Arrange
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new TaskEntity
        {
            TaskId = 1, AssignedBy = 9, AssignedTo = 5, Title = "Prepare report",
            Priority = "High", TaskStatus = "Completed", Description = "Quarterly numbers"
        });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Employee { EmployeeId = 5, EmployeeName = "Alice" });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(new Employee { EmployeeId = 9, EmployeeName = "Bob" });
        _taskCommentRepositoryMock.Setup(r => r.GetByTaskIdAsync(1))
            .ReturnsAsync(new List<TaskComment> { new() { TaskId = 1, EmployeeId = 5, Comment = "All done" } });
        _taskHistoryRepositoryMock.Setup(r => r.GetByTaskIdAsync(1))
            .ReturnsAsync(new List<TaskHistory> { new() { TaskId = 1, Action = "StatusChanged", OldValue = "InProgress", NewValue = "Completed" } });

        // Act
        var result = await _service.GenerateReportAsync(1);

        // Assert
        result.FileName.Should().Be("task-1-report.txt");
        result.ContentType.Should().Be("text/plain");
        var text = System.Text.Encoding.UTF8.GetString(result.Content);
        text.Should().Contain("Prepare report");
        text.Should().Contain("Alice");
        text.Should().Contain("Bob");
        text.Should().Contain("All done");
        text.Should().Contain("StatusChanged");
    }
}
