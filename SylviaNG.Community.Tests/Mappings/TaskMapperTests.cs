using FluentAssertions;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Utils;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Tests.Mappings;

public class TaskMapperTests : IDisposable
{
    public TaskMapperTests()
    {
        // See TaskMapperTests.ToResponse_WhenDueTodayInBusinessTimezone_ShouldNotBeOverdue - reset in
        // Dispose() so this doesn't leak into any other test class in the same process (mirrors
        // LocalDateTimeJsonConverterTests, which hit the same static-singleton issue first).
        DateTimeUtility.Initialize("Asia/Dhaka");
    }

    public void Dispose()
    {
        DateTimeUtility.Initialize("UTC");
    }

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

    /// <summary>
    /// Regression coverage for a real bug: DueDate is a calendar date in the business timezone
    /// (Asia/Dhaka, UTC+6), but the derived-status calculation used to compare raw UTC calendar
    /// dates. Local midnight of "today" converts to 18:00 UTC the previous day, so during the
    /// first six hours of the UTC day (00:00-05:59 UTC = 06:00-11:59 local), a task due "today"
    /// had a UTC due-date one calendar day behind UTC "now" and was wrongly reported Overdue -
    /// exactly what a user hit assigning a task due the same day.
    /// </summary>
    [Fact]
    public void ToResponse_WhenDueTodayInBusinessTimezone_ShouldNotBeOverdue()
    {
        // Arrange
        var todayLocal = DateTimeUtility.TodayLocal();
        var entity = new TaskEntity { TaskStatus = "InProgress", DueDate = DateTimeUtility.StartOfDayUtc(todayLocal), ReminderDays = 1 };

        // Act
        var response = entity.ToResponse();

        // Assert
        response.DerivedStatus.Should().NotBe("Overdue");
    }
}
