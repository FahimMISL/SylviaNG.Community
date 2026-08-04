using System.Linq;
using FluentValidation;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionAdd
{
    public class RecognitionReactionAddValidator : AbstractValidator<RecognitionReactionAddCommand>
    {
        private static readonly string[] AllowedReactionTypes = Enum.GetNames(typeof(ReactionTypeEnum));

        public RecognitionReactionAddValidator()
        {
            RuleFor(x => x.RecognitionId)
                .GreaterThan(0).WithMessage("RecognitionId is required.");

            RuleFor(x => x.CallerEmployeeId)
                .GreaterThan(0).WithMessage("A valid employee identity is required to react.");

            RuleFor(x => x.Request.ReactionType)
                .NotEmpty().WithMessage("ReactionType is required.")
                .Must(rt => AllowedReactionTypes.Contains(rt))
                .WithMessage($"ReactionType must be one of: {string.Join(", ", AllowedReactionTypes)}.");
        }
    }
}
