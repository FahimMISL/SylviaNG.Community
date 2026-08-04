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
            RecipientId = 2,
            RecognitionType = "Peer",
            Message = "Great job!"
        }, callerEmployeeId: 1, isHrOrAdmin: false);

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
            RecipientId = 2,
            RecognitionType = ""
        }, callerEmployeeId: 1, isHrOrAdmin: false);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.RecognitionType");
    }

    [Fact]
    public void Validate_WithZeroCallerEmployeeId_ShouldHaveError()
    {
        // Arrange
        var command = new RecognitionCreateCommand(new RecognitionCreateRequest
        {
            RecipientId = 2,
            RecognitionType = "Peer"
        }, callerEmployeeId: 0, isHrOrAdmin: false);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CallerEmployeeId");
    }
}
