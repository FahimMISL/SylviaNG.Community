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
                .IsInEnum().WithMessage("ReactionType must be a valid value.");
        }
    }
}
