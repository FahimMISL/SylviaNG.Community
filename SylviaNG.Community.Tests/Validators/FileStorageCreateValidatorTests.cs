using FluentAssertions;
using SylviaNG.Community.Application.Features.FileStorages.Commands.FileStorageCreate;
using SylviaNG.Community.Application.Features.FileStorages.Models;

namespace SylviaNG.Community.Tests.Validators;

public class FileStorageCreateValidatorTests
{
    private readonly FileStorageCreateValidator _validator = new();

    private static FileStorageCreateRequest ValidRequest() => new()
    {
        Module = "Team",
        FileName = "abc123.png",
        OriginalFileName = "photo.png",
        StoragePath = "/uploads/abc123.png",
        FileSize = 1024,
        UploadedBy = 1
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new FileStorageCreateCommand(ValidRequest());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyModule_ShouldHaveError()
    {
        // Arrange
        var request = ValidRequest();
        request.Module = "";
        var command = new FileStorageCreateCommand(request);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Module");
    }

    [Fact]
    public void Validate_WithZeroUploadedBy_ShouldHaveError()
    {
        // Arrange
        var request = ValidRequest();
        request.UploadedBy = 0;
        var command = new FileStorageCreateCommand(request);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.UploadedBy");
    }
}
