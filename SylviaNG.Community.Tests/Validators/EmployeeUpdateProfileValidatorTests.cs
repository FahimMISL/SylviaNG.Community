using FluentAssertions;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateProfile;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Domain.Enums;

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

    [Fact]
    public void Validate_WithInvalidEmailFormat_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateProfileCommand(1, new EmployeeUpdateProfileRequest
        {
            Email = "not-an-email"
        }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Email");
    }

    [Fact]
    public void Validate_WithEmptyContactLinkPlatform_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateProfileCommand(1, new EmployeeUpdateProfileRequest
        {
            ContactLinks = new List<EmployeeContactLinkItem>
            {
                new() { Platform = "", Url = "https://linkedin.com/in/ayesha", Visibility = ContactVisibilityEnum.Public }
            }
        }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ContactLinks[0].Platform");
    }

    [Fact]
    public void Validate_WithInvalidContactLinkUrl_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateProfileCommand(1, new EmployeeUpdateProfileRequest
        {
            ContactLinks = new List<EmployeeContactLinkItem>
            {
                new() { Platform = "LinkedIn", Url = "not a url", Visibility = ContactVisibilityEnum.Public }
            }
        }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ContactLinks[0].Url");
    }

    [Fact]
    public void Validate_WithContactLinkVisibilityOutOfEnumRange_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateProfileCommand(1, new EmployeeUpdateProfileRequest
        {
            ContactLinks = new List<EmployeeContactLinkItem>
            {
                new() { Platform = "LinkedIn", Url = "https://linkedin.com/in/ayesha", Visibility = (ContactVisibilityEnum)99 }
            }
        }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ContactLinks[0].Visibility");
    }

    [Fact]
    public void Validate_WithValidMixedAddUpdateContactLinks_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new EmployeeUpdateProfileCommand(1, new EmployeeUpdateProfileRequest
        {
            Phone = "+880-1710-000001",
            Email = "ayesha.rahman@sylviang.example",
            Extension = "1001",
            ContactLinks = new List<EmployeeContactLinkItem>
            {
                new() { Id = 10, Platform = "LinkedIn", Url = "https://linkedin.com/in/ayesha", Visibility = ContactVisibilityEnum.Public },
                new() { Id = null, Platform = "GitHub", Url = "https://github.com/ayesha", Visibility = ContactVisibilityEnum.Private }
            }
        }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
