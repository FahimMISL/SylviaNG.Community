using FluentAssertions;
using SylviaNG.Community.Application.Features.Notifications.Commands.NotificationCreate;
using SylviaNG.Community.Application.Features.Notifications.Models;

namespace SylviaNG.Community.Tests.Validators;

public class NotificationCreateValidatorTests
{
    private readonly NotificationCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new NotificationCreateCommand(new NotificationCreateRequest { EmployeeId = 1, Title = "Welcome" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldHaveError()
    {
        // Arrange
        var command = new NotificationCreateCommand(new NotificationCreateRequest { EmployeeId = 1, Title = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Title");
    }

    [Fact]
    public void Validate_WithZeroEmployeeId_ShouldHaveError()
    {
        // Arrange
        var command = new NotificationCreateCommand(new NotificationCreateRequest { EmployeeId = 0, Title = "Welcome" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.EmployeeId");
    }
}
