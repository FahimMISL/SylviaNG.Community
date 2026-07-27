using FluentAssertions;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamCreate;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Tests.Validators;

public class TeamCreateValidatorTests
{
    private readonly TeamCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new TeamCreateCommand(new TeamCreateRequest { Name = "Engineering" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new TeamCreateCommand(new TeamCreateRequest { Name = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Name");
    }
}
