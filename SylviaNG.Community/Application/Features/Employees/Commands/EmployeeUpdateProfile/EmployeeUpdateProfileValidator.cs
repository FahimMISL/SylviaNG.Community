using FluentValidation;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateProfile
{
    public class EmployeeUpdateProfileValidator : AbstractValidator<EmployeeUpdateProfileCommand>
    {
        public EmployeeUpdateProfileValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.Bio)
                .MaximumLength(2000).WithMessage("Bio must not exceed 2000 characters.");

            RuleFor(x => x.Request.Skills)
                .MaximumLength(1000).WithMessage("Skills must not exceed 1000 characters.");

            RuleFor(x => x.Request.Interests)
                .MaximumLength(1000).WithMessage("Interests must not exceed 1000 characters.");

            RuleFor(x => x.Request.Achievements)
                .MaximumLength(1000).WithMessage("Achievements must not exceed 1000 characters.");

            RuleFor(x => x.Request.CommunityContributions)
                .MaximumLength(1000).WithMessage("Community contributions must not exceed 1000 characters.");
        }
    }
}
