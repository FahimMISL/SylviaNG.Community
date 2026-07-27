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
                .NotEmpty().WithMessage("ReactionType is required.")
                .MaximumLength(50).WithMessage("ReactionType must not exceed 50 characters.");
        }
    }
}
