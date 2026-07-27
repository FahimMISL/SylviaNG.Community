using FluentValidation;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberAdd
{
    public class TeamMemberAddValidator : AbstractValidator<TeamMemberAddCommand>
    {
        public TeamMemberAddValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");
        }
    }
}
