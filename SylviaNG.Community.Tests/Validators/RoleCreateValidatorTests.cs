using FluentAssertions;
using SylviaNG.Community.Application.Features.Roles.Commands.RoleCreate;
using SylviaNG.Community.Application.Features.Roles.Models;

namespace SylviaNG.Community.Tests.Validators;

public class RoleCreateValidatorTests
{
    private readonly RoleCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new RoleCreateCommand(new RoleCreateRequest { Name = "HR" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new RoleCreateCommand(new RoleCreateRequest { Name = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Name");
    }
}
