using FluentAssertions;
using SylviaNG.Community.Application.Features.DashboardPreferences.Commands.DashboardPreferenceUpsert;
using SylviaNG.Community.Application.Features.DashboardPreferences.Models;

namespace SylviaNG.Community.Tests.Validators;

public class DashboardPreferenceUpsertValidatorTests
{
    private readonly DashboardPreferenceUpsertValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new DashboardPreferenceUpsertCommand(new DashboardPreferenceUpsertRequest { EmployeeId = 1, WidgetName = "TeamRoster", DisplayOrder = 1 });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyWidgetName_ShouldHaveError()
    {
        // Arrange
        var command = new DashboardPreferenceUpsertCommand(new DashboardPreferenceUpsertRequest { EmployeeId = 1, WidgetName = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.WidgetName");
    }

    [Fact]
    public void Validate_WithNegativeDisplayOrder_ShouldHaveError()
    {
        // Arrange
        var command = new DashboardPreferenceUpsertCommand(new DashboardPreferenceUpsertRequest { EmployeeId = 1, WidgetName = "TeamRoster", DisplayOrder = -1 });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.DisplayOrder");
    }
}
