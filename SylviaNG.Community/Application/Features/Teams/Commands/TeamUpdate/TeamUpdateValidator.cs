using FluentValidation;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamUpdate
{
    public class TeamUpdateValidator : AbstractValidator<TeamUpdateCommand>
    {
        public TeamUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(x => x.Request.Name != null);
        }
    }
}
