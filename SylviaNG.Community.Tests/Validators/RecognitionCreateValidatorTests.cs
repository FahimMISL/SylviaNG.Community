using FluentAssertions;
using SylviaNG.Community.Application.Features.Recognitions.Commands.RecognitionCreate;
using SylviaNG.Community.Application.Features.Recognitions.Models;

namespace SylviaNG.Community.Tests.Validators;

public class RecognitionCreateValidatorTests
{
    private readonly RecognitionCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new RecognitionCreateCommand(new RecognitionCreateRequest
        {
            SenderId = 1,
            RecipientId = 2,
            RecognitionType = "Peer",
            Message = "Great job!"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMissingRecognitionType_ShouldHaveError()
    {
        // Arrange
        var command = new RecognitionCreateCommand(new RecognitionCreateRequest
        {
            SenderId = 1,
            RecipientId = 2,
            RecognitionType = ""
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.RecognitionType");
    }

    [Fact]
    public void Validate_WithZeroSenderId_ShouldHaveError()
    {
        // Arrange
        var command = new RecognitionCreateCommand(new RecognitionCreateRequest
        {
            SenderId = 0,
            RecipientId = 2,
            RecognitionType = "Peer"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.SenderId");
    }
}
