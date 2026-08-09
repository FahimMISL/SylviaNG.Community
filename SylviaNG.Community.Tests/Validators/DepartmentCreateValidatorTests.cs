using FluentAssertions;
using SylviaNG.Community.Application.Features.Departments.Commands.DepartmentCreate;
using SylviaNG.Community.Application.Features.Departments.Models;

namespace SylviaNG.Community.Tests.Validators;

public class DepartmentCreateValidatorTests
{
    private readonly DepartmentCreateValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new DepartmentCreateCommand(new DepartmentCreateRequest { Name = "Engineering" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new DepartmentCreateCommand(new DepartmentCreateRequest { Name = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Name");
    }
}
