using FluentValidation;

namespace SylviaNG.Community.Application.Features.CommentReactions.Commands.CommentReactionAdd
{
    public class CommentReactionAddValidator : AbstractValidator<CommentReactionAddCommand>
    {
        public CommentReactionAddValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.ReactionType)
                .NotEmpty().WithMessage("ReactionType is required.")
                .MaximumLength(50).WithMessage("ReactionType must not exceed 50 characters.");
        }
    }
}
