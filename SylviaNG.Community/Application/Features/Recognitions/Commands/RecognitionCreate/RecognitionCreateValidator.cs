using FluentValidation;

namespace SylviaNG.Community.Application.Features.Recognitions.Commands.RecognitionCreate
{
    public class RecognitionCreateValidator : AbstractValidator<RecognitionCreateCommand>
    {
        public RecognitionCreateValidator()
        {
            RuleFor(x => x.CallerEmployeeId)
                .GreaterThan(0).WithMessage("A valid employee identity is required to send a recognition.");

            RuleFor(x => x.Request.RecipientId)
                .GreaterThan(0).WithMessage("RecipientId is required.");

            RuleFor(x => x.Request.BadgeId)
                .GreaterThan(0).WithMessage("BadgeId must be a positive id.")
                .When(x => x.Request.BadgeId.HasValue);

            RuleFor(x => x.Request.RecognitionType)
                .NotEmpty().WithMessage("RecognitionType is required.")
                .MaximumLength(100).WithMessage("RecognitionType must not exceed 100 characters.");

            RuleFor(x => x.Request.CoreValue)
                .MaximumLength(100).WithMessage("CoreValue must not exceed 100 characters.");

            RuleFor(x => x.Request.AwardTitle)
                .MaximumLength(200).WithMessage("AwardTitle must not exceed 200 characters.");
        }
    }
}
