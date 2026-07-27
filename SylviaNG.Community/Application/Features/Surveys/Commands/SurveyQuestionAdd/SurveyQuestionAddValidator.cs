using FluentValidation;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionAdd
{
    public class SurveyQuestionAddValidator : AbstractValidator<SurveyQuestionAddCommand>
    {
        public SurveyQuestionAddValidator()
        {
            RuleFor(x => x.Request.QuestionText)
                .NotEmpty().WithMessage("QuestionText is required.");

            RuleFor(x => x.Request.QuestionType)
                .NotEmpty().WithMessage("QuestionType is required.")
                .MaximumLength(50).WithMessage("QuestionType must not exceed 50 characters.");

            RuleForEach(x => x.Request.Options)
                .ChildRules(option =>
                {
                    option.RuleFor(o => o.OptionText)
                        .NotEmpty().WithMessage("OptionText is required.")
                        .MaximumLength(500).WithMessage("OptionText must not exceed 500 characters.");
                });
        }
    }
}
