using FluentAssertions;
using SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentAdd;
using SylviaNG.Community.Application.Features.PostComments.Models;

namespace SylviaNG.Community.Tests.Validators;

public class PostCommentAddValidatorTests
{
    private readonly PostCommentAddValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new PostCommentAddCommand(1, new PostCommentAddRequest { EmployeeId = 2, Content = "Nice!" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMissingEmployeeId_ShouldHaveError()
    {
        // Arrange
        var command = new PostCommentAddCommand(1, new PostCommentAddRequest { EmployeeId = 0, Content = "Nice!" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.EmployeeId");
    }

    [Fact]
    public void Validate_WithEmptyContent_ShouldHaveError()
    {
        // Arrange
        var command = new PostCommentAddCommand(1, new PostCommentAddRequest { EmployeeId = 2, Content = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Content");
    }
}
