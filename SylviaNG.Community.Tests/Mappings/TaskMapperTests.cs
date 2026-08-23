using FluentAssertions;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Mappings;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Tests.Mappings;

public class TaskMapperTests
{
    [Fact]
    public void ToEntity_WhenReminderDaysNotProvided_ShouldDefaultToTwo()
    {
        // Arrange
        var request = new TaskCreateRequest { AssignedBy = 1, AssignedTo = 2, Title = "T", Priority = "High", Status = "Open" };

        // Act
        var entity = request.ToEntity();

        // Assert
        entity.ReminderDays.Should().Be(2);
    }

    [Fact]
    public void ToEntity_WhenReminderDaysProvided_ShouldKeepIt()
    {
        // Arrange
        var request = new TaskCreateRequest { AssignedBy = 1, AssignedTo = 2, Title = "T", Priority = "High", Status = "Open", ReminderDays = 5 };

        // Act
        var entity = request.ToEntity();

        // Assert
        entity.ReminderDays.Should().Be(5);
    }

    [Fact]
    public void ToResponse_WhenStatusIsCompleted_ShouldBeCompletedRegardlessOfDueDate()
    {
        // Arrange
        var entity = new TaskEntity { TaskStatus = "Completed", DueDate = DateTime.UtcNow.AddDays(-10) };

        // Act
        var response = entity.ToResponse();

        // Assert
        response.DerivedStatus.Should().Be("Completed");
    }

    [Fact]
    public void ToResponse_WhenDueDateIsPast_ShouldBeOverdue()
    {
        // Arrange
        var entity = new TaskEntity { TaskStatus = "InProgress", DueDate = DateTime.UtcNow.AddDays(-1), ReminderDays = 2 };

        // Act
        var response = entity.ToResponse();

        // Assert
        response.DerivedStatus.Should().Be("Overdue");
    }

    [Fact]
    public void ToResponse_WhenWithinReminderWindow_ShouldBeDueSoon()
    {
        // Arrange
        var entity = new TaskEntity { TaskStatus = "InProgress", DueDate = DateTime.UtcNow.AddHours(12), ReminderDays = 2 };

        // Act
        var response = entity.ToResponse();

        // Assert
        response.DerivedStatus.Should().Be("DueSoon");
    }

    [Fact]
    public void ToResponse_WhenFarFromDueDate_ShouldBeOnTrack()
    {
        // Arrange
        var entity = new TaskEntity { TaskStatus = "InProgress", DueDate = DateTime.UtcNow.AddDays(10), ReminderDays = 2 };

        // Act
        var response = entity.ToResponse();

        // Assert
        response.DerivedStatus.Should().Be("OnTrack");
    }

    [Fact]
    public void ToResponse_WhenNoDueDate_ShouldBeOnTrack()
    {
        // Arrange
        var entity = new TaskEntity { TaskStatus = "Assigned", DueDate = null };

        // Act
        var response = entity.ToResponse();

        // Assert
        response.DerivedStatus.Should().Be("OnTrack");
    }
}
