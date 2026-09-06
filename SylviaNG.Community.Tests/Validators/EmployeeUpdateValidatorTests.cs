using FluentAssertions;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdate;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.SharedKernel.Utils;

namespace SylviaNG.Community.Tests.Validators;

public class EmployeeUpdateValidatorTests
{
    private readonly EmployeeUpdateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new EmployeeUpdateCommand(1, new EmployeeUpdateRequest
        {
            Email = "ayesha.rahman@sylviang.example",
            DateOfBirth = DateTimeUtility.TodayLocal().AddYears(-30),
            DateOfJoining = DateTimeUtility.TodayLocal().AddYears(-2)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithZeroEmployeeId_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateCommand(0, new EmployeeUpdateRequest { Email = "ayesha.rahman@sylviang.example" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "EmployeeId");
    }

    [Fact]
    public void Validate_WithEmptyEmail_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateCommand(1, new EmployeeUpdateRequest { Email = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Email");
    }

    [Fact]
    public void Validate_WithInvalidEmailFormat_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateCommand(1, new EmployeeUpdateRequest { Email = "not-an-email" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Email");
    }

    [Fact]
    public void Validate_WithNullDateOfBirth_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new EmployeeUpdateCommand(1, new EmployeeUpdateRequest
        {
            Email = "ayesha.rahman@sylviang.example",
            DateOfBirth = null,
            DateOfJoining = DateTimeUtility.TodayLocal()
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithFutureDateOfBirth_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateCommand(1, new EmployeeUpdateRequest
        {
            Email = "ayesha.rahman@sylviang.example",
            DateOfBirth = DateTimeUtility.TodayLocal().AddDays(1),
            DateOfJoining = DateTimeUtility.TodayLocal()
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.DateOfBirth");
    }

    [Fact]
    public void Validate_WithUnreasonablyOldDateOfBirth_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateCommand(1, new EmployeeUpdateRequest
        {
            Email = "ayesha.rahman@sylviang.example",
            DateOfBirth = DateTimeUtility.TodayLocal().AddYears(-150),
            DateOfJoining = DateTimeUtility.TodayLocal()
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.DateOfBirth");
    }

    [Fact]
    public void Validate_WithFutureDateOfJoining_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeUpdateCommand(1, new EmployeeUpdateRequest
        {
            Email = "ayesha.rahman@sylviang.example",
            DateOfJoining = DateTimeUtility.TodayLocal().AddDays(1)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.DateOfJoining");
    }
}
