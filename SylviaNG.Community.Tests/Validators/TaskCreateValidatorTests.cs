using FluentAssertions;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskCreate;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Tests.Validators;

public class TaskCreateValidatorTests
{
    private readonly TaskCreateValidator _validator = new();

    private static TaskCreateRequest ValidRequest() => new()
    {
        TeamId = 1,
        AssignedBy = 2,
        AssignedTo = 3,
        Title = "Prepare report",
        Priority = "High",
        Status = "Open"
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new TaskCreateCommand(ValidRequest());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldHaveError()
    {
        // Arrange
        var request = ValidRequest();
        request.Title = "";
        var command = new TaskCreateCommand(request);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Title");
    }

    [Fact]
    public void Validate_WithZeroTeamId_ShouldHaveError()
    {
        // Arrange
        var request = ValidRequest();
        request.TeamId = 0;
        var command = new TaskCreateCommand(request);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.TeamId");
    }

    [Fact]
    public void Validate_WithZeroAssignedTo_ShouldHaveError()
    {
        // Arrange
        var request = ValidRequest();
        request.AssignedTo = 0;
        var command = new TaskCreateCommand(request);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.AssignedTo");
    }

    [Fact]
    public void Validate_WithEmptyPriority_ShouldHaveError()
    {
        // Arrange
        var request = ValidRequest();
        request.Priority = "";
        var command = new TaskCreateCommand(request);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Priority");
    }

    [Fact]
    public void Validate_WithNegativeReminderDays_ShouldHaveError()
    {
        // Arrange
        var request = ValidRequest();
        request.ReminderDays = -1;
        var command = new TaskCreateCommand(request);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ReminderDays");
    }
}
