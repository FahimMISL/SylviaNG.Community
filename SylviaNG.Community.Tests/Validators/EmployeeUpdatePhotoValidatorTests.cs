using FluentAssertions;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdatePhoto;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Tests.Validators;

public class EmployeeUpdatePhotoValidatorTests
{
    private readonly EmployeeUpdatePhotoValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new EmployeeUpdatePhotoCommand(1, new EmployeeUpdatePhotoRequest { StoragePath = "uploads/employee-photo/2026-07/guid.jpg" }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyStoragePath_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdatePhotoCommand(1, new EmployeeUpdatePhotoRequest { StoragePath = "" }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.StoragePath");
    }

    [Fact]
    public void Validate_WithStoragePathNotStartingWithUploads_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdatePhotoCommand(1, new EmployeeUpdatePhotoRequest { StoragePath = "http://evil.example/x.jpg" }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.StoragePath");
    }

    [Fact]
    public void Validate_WithZeroEmployeeId_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdatePhotoCommand(0, new EmployeeUpdatePhotoRequest { StoragePath = "uploads/employee-photo/2026-07/guid.jpg" }, viewerEmployeeId: 1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "EmployeeId");
    }
}
