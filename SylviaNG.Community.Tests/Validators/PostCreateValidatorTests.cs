using FluentAssertions;
using SylviaNG.Community.Application.Features.Posts.Commands.PostCreate;
using SylviaNG.Community.Application.Features.Posts.Models;

namespace SylviaNG.Community.Tests.Validators;

public class PostCreateValidatorTests
{
    private readonly PostCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new PostCreateCommand(new PostCreateRequest { EmployeeId = 1, Type = "Update", Visibility = "Public" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMissingEmployeeId_ShouldHaveError()
    {
        // Arrange
        var command = new PostCreateCommand(new PostCreateRequest { EmployeeId = 0, Type = "Update", Visibility = "Public" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.EmployeeId");
    }

    [Fact]
    public void Validate_WithEmptyType_ShouldHaveError()
    {
        // Arrange
        var command = new PostCreateCommand(new PostCreateRequest { EmployeeId = 1, Type = "", Visibility = "Public" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Type");
    }

    [Fact]
    public void Validate_WithEmptyVisibility_ShouldHaveError()
    {
        // Arrange
        var command = new PostCreateCommand(new PostCreateRequest { EmployeeId = 1, Type = "Update", Visibility = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Visibility");
    }
}
