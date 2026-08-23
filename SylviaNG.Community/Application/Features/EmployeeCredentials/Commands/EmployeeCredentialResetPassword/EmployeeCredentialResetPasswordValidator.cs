using FluentValidation;

namespace SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialResetPassword
{
    public class EmployeeCredentialResetPasswordValidator : AbstractValidator<EmployeeCredentialResetPasswordCommand>
    {
        public EmployeeCredentialResetPasswordValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.TemporaryPassword)
                .NotEmpty().WithMessage("TemporaryPassword is required.")
                .MinimumLength(8).WithMessage("TemporaryPassword must be at least 8 characters.")
                .Matches("[A-Za-z]").WithMessage("TemporaryPassword must contain at least one letter.")
                .Matches("[0-9]").WithMessage("TemporaryPassword must contain at least one digit.");
        }
    }
}
