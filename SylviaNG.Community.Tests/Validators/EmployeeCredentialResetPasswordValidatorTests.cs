using FluentAssertions;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialResetPassword;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;

namespace SylviaNG.Community.Tests.Validators;

public class EmployeeCredentialResetPasswordValidatorTests
{
    private readonly EmployeeCredentialResetPasswordValidator _validator = new();

    private static EmployeeCredentialResetPasswordCommand ValidCommand(long employeeId = 1) => new(employeeId, new EmployeeCredentialResetPasswordRequest
    {
        TemporaryPassword = "NewTemp1234"
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
        var command = ValidCommand(0);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "EmployeeId");
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
}
