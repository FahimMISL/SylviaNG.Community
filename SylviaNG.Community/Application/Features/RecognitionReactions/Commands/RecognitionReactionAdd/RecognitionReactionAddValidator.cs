using FluentValidation;

namespace SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionAdd
{
    public class RecognitionReactionAddValidator : AbstractValidator<RecognitionReactionAddCommand>
    {
        public RecognitionReactionAddValidator()
        {
            RuleFor(x => x.RecognitionId)
                .GreaterThan(0).WithMessage("RecognitionId is required.");

            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.ReactionType)
                .NotEmpty().WithMessage("ReactionType is required.")
                .MaximumLength(50).WithMessage("ReactionType must not exceed 50 characters.");
        }
    }
}
