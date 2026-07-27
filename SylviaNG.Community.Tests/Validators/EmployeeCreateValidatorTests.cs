using FluentAssertions;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeCreate;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Tests.Validators;

public class EmployeeCreateValidatorTests
{
    private readonly EmployeeCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new EmployeeCreateCommand(new EmployeeCreateRequest
        {
            EmployeeName = "Ayesha Rahman",
            Email = "ayesha.rahman@sylviang.example",
            DesignationId = 1,
            DepartmentId = 1,
            SiteId = 1
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeCreateCommand(new EmployeeCreateRequest
        {
            EmployeeName = "",
            Email = "ayesha.rahman@sylviang.example",
            DesignationId = 1,
            DepartmentId = 1,
            SiteId = 1
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.EmployeeName");
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeCreateCommand(new EmployeeCreateRequest
        {
            EmployeeName = "Ayesha Rahman",
            Email = "not-an-email",
            DesignationId = 1,
            DepartmentId = 1,
            SiteId = 1
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Email");
    }

    [Fact]
    public void Validate_WithZeroDepartmentId_ShouldHaveError()
    {
        // Arrange
        var command = new EmployeeCreateCommand(new EmployeeCreateRequest
        {
            EmployeeName = "Ayesha Rahman",
            Email = "ayesha.rahman@sylviang.example",
            DesignationId = 1,
            DepartmentId = 0,
            SiteId = 1
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.DepartmentId");
    }
}
