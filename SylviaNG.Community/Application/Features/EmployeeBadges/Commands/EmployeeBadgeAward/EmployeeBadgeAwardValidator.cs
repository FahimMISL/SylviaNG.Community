using FluentValidation;

namespace SylviaNG.Community.Application.Features.EmployeeBadges.Commands.EmployeeBadgeAward
{
    public class EmployeeBadgeAwardValidator : AbstractValidator<EmployeeBadgeAwardCommand>
    {
        public EmployeeBadgeAwardValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.BadgeId)
                .GreaterThan(0).WithMessage("BadgeId is required.");
        }
    }
}
