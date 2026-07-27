using FluentValidation;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionAudienceTargetAdd
{
    public class ElectionAudienceTargetAddValidator : AbstractValidator<ElectionAudienceTargetAddCommand>
    {
        public ElectionAudienceTargetAddValidator()
        {
            RuleFor(x => x.Request.TargetId)
                .NotEmpty().WithMessage("TargetId is required.")
                .MaximumLength(200).WithMessage("TargetId must not exceed 200 characters.");
        }
    }
}
