using FluentAssertions;
using SylviaNG.Community.Application.Features.Designations.Commands.DesignationCreate;
using SylviaNG.Community.Application.Features.Designations.Models;

namespace SylviaNG.Community.Tests.Validators;

public class DesignationCreateValidatorTests
{
    private readonly DesignationCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new DesignationCreateCommand(new DesignationCreateRequest { Name = "Software Engineer" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new DesignationCreateCommand(new DesignationCreateRequest { Name = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Name");
    }
}
