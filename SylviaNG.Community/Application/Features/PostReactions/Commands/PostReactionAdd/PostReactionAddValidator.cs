using FluentValidation;

namespace SylviaNG.Community.Application.Features.PostReactions.Commands.PostReactionAdd
{
    public class PostReactionAddValidator : AbstractValidator<PostReactionAddCommand>
    {
        public PostReactionAddValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.ReactionType)
                .IsInEnum().WithMessage("ReactionType must be a valid value.");
        }
    }
}
