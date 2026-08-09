using FluentAssertions;
using SylviaNG.Community.Application.Features.Branches.Commands.BranchCreate;
using SylviaNG.Community.Application.Features.Branches.Models;

namespace SylviaNG.Community.Tests.Validators;

public class BranchCreateValidatorTests
{
    private readonly BranchCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new BranchCreateCommand(new BranchCreateRequest { Name = "Head Office" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new BranchCreateCommand(new BranchCreateRequest { Name = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Name");
    }
}
