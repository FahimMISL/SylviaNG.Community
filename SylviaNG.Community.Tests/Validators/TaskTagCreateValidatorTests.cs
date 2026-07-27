using FluentAssertions;
using SylviaNG.Community.Application.Features.TaskTags.Commands.TaskTagCreate;
using SylviaNG.Community.Application.Features.TaskTags.Models;

namespace SylviaNG.Community.Tests.Validators;

public class TaskTagCreateValidatorTests
{
    private readonly TaskTagCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new TaskTagCreateCommand(new TaskTagCreateRequest { Name = "Urgent" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new TaskTagCreateCommand(new TaskTagCreateRequest { Name = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
