using FluentAssertions;
using SylviaNG.Community.Application.Features.Notifications.Commands.NotificationPreferenceUpsert;
using SylviaNG.Community.Application.Features.Notifications.Models;

namespace SylviaNG.Community.Tests.Validators;

public class NotificationPreferenceUpsertValidatorTests
{
    private readonly NotificationPreferenceUpsertValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new NotificationPreferenceUpsertCommand(new NotificationPreferenceUpsertRequest { EmployeeId = 1, Category = "Announcements" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCategory_ShouldHaveError()
    {
        // Arrange
        var command = new NotificationPreferenceUpsertCommand(new NotificationPreferenceUpsertRequest { EmployeeId = 1, Category = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Category");
    }
}
