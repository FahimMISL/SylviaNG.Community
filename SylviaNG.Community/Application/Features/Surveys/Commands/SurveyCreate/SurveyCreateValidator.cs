using FluentValidation;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyCreate
{
    public class SurveyCreateValidator : AbstractValidator<SurveyCreateCommand>
    {
        public SurveyCreateValidator()
        {
            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Request.SurveyType)
                .NotEmpty().WithMessage("SurveyType is required.")
                .MaximumLength(50).WithMessage("SurveyType must not exceed 50 characters.");

            RuleFor(x => x.Request.ExternalUrl)
                .MaximumLength(2048).WithMessage("ExternalUrl must not exceed 2048 characters.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("ExternalUrl must be a valid absolute URL.")
                .When(x => !string.IsNullOrEmpty(x.Request.ExternalUrl));
        }
    }
}
