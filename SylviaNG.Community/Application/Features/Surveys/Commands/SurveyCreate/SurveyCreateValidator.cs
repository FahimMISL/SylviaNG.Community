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
        }
    }
}
