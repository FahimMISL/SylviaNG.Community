using FluentAssertions;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateProfile;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Tests.Validators;

public class EmployeeUpdateProfileValidatorTests
{
    private readonly EmployeeUpdateProfileValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new EmployeeUpdateProfileCommand(1, new EmployeeUpdateProfileRequest
        {
            Bio = "Frontend engineer.",
            Skills = "Angular, TypeScript",
            Interests = "Chess",
            Achievements = "Employee of the month",
            CommunityContributions = "Mentored 3 interns"
        }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithAchievementsOverMaxLength_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateProfileCommand(1, new EmployeeUpdateProfileRequest
        {
            Achievements = new string('a', 1001)
        }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Achievements");
    }

    [Fact]
    public void Validate_WithCommunityContributionsOverMaxLength_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateProfileCommand(1, new EmployeeUpdateProfileRequest
        {
            CommunityContributions = new string('a', 1001)
        }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.CommunityContributions");
    }

    [Fact]
    public void Validate_WithBioOverMaxLength_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateProfileCommand(1, new EmployeeUpdateProfileRequest
        {
            Bio = new string('a', 2001)
        }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Bio");
    }

    [Fact]
    public void Validate_WithZeroEmployeeId_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateProfileCommand(0, new EmployeeUpdateProfileRequest(), viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "EmployeeId");
    }
}
