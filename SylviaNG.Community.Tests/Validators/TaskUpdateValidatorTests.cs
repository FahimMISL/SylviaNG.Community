using FluentAssertions;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskUpdate;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Tests.Validators;

public class TaskUpdateValidatorTests
{
    private readonly TaskUpdateValidator _validator = new();

    private static TaskUpdateCommand Command(int? reminderDays) =>
        new(1, new TaskUpdateRequest { ReminderDays = reminderDays }, changedBy: 1, isHrOrAdmin: true);

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    public void Validate_WithReminderDaysOutsideOneToFourteen_ShouldHaveError(int reminderDays)
    {
        // Act
        var result = _validator.Validate(Command(reminderDays));

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ReminderDays");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(14)]
    public void Validate_WithReminderDaysAtBoundary_ShouldHaveNoErrors(int reminderDays)
    {
        // Act
        var result = _validator.Validate(Command(reminderDays));

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithReminderDaysNotProvided_ShouldHaveNoErrors()
    {
        // Act
        var result = _validator.Validate(Command(null));

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
