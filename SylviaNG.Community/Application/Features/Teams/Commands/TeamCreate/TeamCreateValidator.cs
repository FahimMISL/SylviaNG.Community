using FluentValidation;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamCreate
{
    public class TeamCreateValidator : AbstractValidator<TeamCreateCommand>
    {
        public TeamCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        }
    }
}
