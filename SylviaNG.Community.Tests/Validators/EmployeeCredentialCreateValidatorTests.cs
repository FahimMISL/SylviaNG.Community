using FluentAssertions;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialCreate;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;

namespace SylviaNG.Community.Tests.Validators;

public class EmployeeCredentialCreateValidatorTests
{
    private readonly EmployeeCredentialCreateValidator _validator = new();

    private static EmployeeCredentialCreateCommand ValidCommand() => new(new EmployeeCredentialCreateRequest
    {
        EmployeeId = 1,
        Username = "ayesha.rahman",
        TemporaryPassword = "Temp1234"
    });

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithZeroEmployeeId_ShouldHaveError()
    {
        var command = ValidCommand();
        command.Request.EmployeeId = 0;

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.EmployeeId");
    }

    [Fact]
    public void Validate_WithEmptyUsername_ShouldHaveError()
    {
        var command = ValidCommand();
        command.Request.Username = "";

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Username");
    }

    [Fact]
    public void Validate_WithTooShortPassword_ShouldHaveError()
    {
        var command = ValidCommand();
        command.Request.TemporaryPassword = "Ab1";

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.TemporaryPassword");
    }

    [Fact]
    public void Validate_WithPasswordMissingDigit_ShouldHaveError()
    {
        var command = ValidCommand();
        command.Request.TemporaryPassword = "Passwordonly";

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.TemporaryPassword");
    }

    [Fact]
    public void Validate_WithDisallowedRole_ShouldHaveError()
    {
        var command = ValidCommand();
        command.Request.Role = "SuperUser";

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Role");
    }

    [Fact]
    public void Validate_WithAllowedRole_ShouldHaveNoErrors()
    {
        var command = ValidCommand();
        command.Request.Role = "HR";

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
