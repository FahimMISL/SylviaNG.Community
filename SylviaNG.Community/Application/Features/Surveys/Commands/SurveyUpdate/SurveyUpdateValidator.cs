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

            RuleFor(x => x.Request.ExternalUrl)
                .MaximumLength(2048).WithMessage("ExternalUrl must not exceed 2048 characters.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("ExternalUrl must be a valid absolute URL.")
                .When(x => !string.IsNullOrEmpty(x.Request.ExternalUrl));
        }
    }
}
