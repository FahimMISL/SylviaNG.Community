using FluentValidation;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyUpdate
{
    public class SurveyUpdateValidator : AbstractValidator<SurveyUpdateCommand>
    {
        public SurveyUpdateValidator()
        {
            RuleFor(x => x.Request.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .When(x => x.Request.Title != null);

            RuleFor(x => x.Request.SurveyType)
                .MaximumLength(50).WithMessage("SurveyType must not exceed 50 characters.")
                .When(x => x.Request.SurveyType != null);
        }
    }
}
