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
                .Must(t => SurveyQuestionTypes.All.Contains(t))
                .WithMessage($"QuestionType must be one of: {string.Join(", ", SurveyQuestionTypes.All)}.");

            RuleFor(x => x.Request.Options)
                .Must(options => options.Count > 0)
                .When(x => SurveyQuestionTypes.ChoiceTypes.Contains(x.Request.QuestionType))
                .WithMessage("Choice-type questions require at least one option.");

            RuleFor(x => x.Request.Options)
                .Must(options => options
                    .Select(o => o.OptionText.Trim().ToLowerInvariant())
                    .Distinct()
                    .Count() == options.Count)
                .WithMessage("Duplicate option text is not allowed within a question.");

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
