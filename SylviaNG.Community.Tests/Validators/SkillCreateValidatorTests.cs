using FluentAssertions;
using SylviaNG.Community.Application.Features.Skills.Commands.SkillCreate;
using SylviaNG.Community.Application.Features.Skills.Models;

namespace SylviaNG.Community.Tests.Validators;

public class SkillCreateValidatorTests
{
    private readonly SkillCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new SkillCreateCommand(new SkillCreateRequest { Name = "C#" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new SkillCreateCommand(new SkillCreateRequest { Name = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Name");
    }

    [Fact]
    public void Validate_WithNameTooLong_ShouldHaveError()
    {
        // Arrange
        var command = new SkillCreateCommand(new SkillCreateRequest { Name = new string('a', 201) });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
