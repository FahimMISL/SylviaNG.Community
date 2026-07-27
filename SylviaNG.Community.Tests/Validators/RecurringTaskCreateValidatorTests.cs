using FluentAssertions;
using SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskCreate;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;

namespace SylviaNG.Community.Tests.Validators;

public class RecurringTaskCreateValidatorTests
{
    private readonly RecurringTaskCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new RecurringTaskCreateCommand(new RecurringTaskCreateRequest
        {
            Frequency = "Weekly",
            IntervalValue = 1,
            StartDate = DateTime.UtcNow
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithZeroIntervalValue_ShouldHaveError()
    {
        // Arrange
        var command = new RecurringTaskCreateCommand(new RecurringTaskCreateRequest
        {
            Frequency = "Weekly",
            IntervalValue = 0,
            StartDate = DateTime.UtcNow
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
