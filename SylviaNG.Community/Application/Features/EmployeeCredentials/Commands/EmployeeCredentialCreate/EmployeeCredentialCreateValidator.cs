using FluentValidation;

namespace SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialCreate
{
    public class EmployeeCredentialCreateValidator : AbstractValidator<EmployeeCredentialCreateCommand>
    {
        private static readonly string[] AllowedRoles = { "Employee", "Supervisor", "HR", "Admin" };

        public EmployeeCredentialCreateValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(100).WithMessage("Username must not exceed 100 characters.");

            RuleFor(x => x.Request.TemporaryPassword)
                .NotEmpty().WithMessage("TemporaryPassword is required.")
                .MinimumLength(8).WithMessage("TemporaryPassword must be at least 8 characters.")
                .Matches("[A-Za-z]").WithMessage("TemporaryPassword must contain at least one letter.")
                .Matches("[0-9]").WithMessage("TemporaryPassword must contain at least one digit.");

            RuleFor(x => x.Request.Role)
                .Must(role => AllowedRoles.Contains(role))
                .When(x => x.Request.Role != null)
                .WithMessage($"Role must be one of: {string.Join(", ", AllowedRoles)}.");
        }
    }
}
