using FluentValidation;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyAudienceAdd
{
    public class SurveyAudienceAddValidator : AbstractValidator<SurveyAudienceAddCommand>
    {
        public SurveyAudienceAddValidator()
        {
            RuleFor(x => x.Request.AudienceType)
                .NotEmpty().WithMessage("AudienceType is required.")
                .MaximumLength(50).WithMessage("AudienceType must not exceed 50 characters.");
        }
    }
}
