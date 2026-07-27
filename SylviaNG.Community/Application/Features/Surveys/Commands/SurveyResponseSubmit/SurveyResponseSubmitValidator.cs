using FluentValidation;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyResponseSubmit
{
    public class SurveyResponseSubmitValidator : AbstractValidator<SurveyResponseSubmitCommand>
    {
        public SurveyResponseSubmitValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.Answers)
                .NotEmpty().WithMessage("At least one answer is required.");

            RuleForEach(x => x.Request.Answers)
                .ChildRules(answer =>
                {
                    answer.RuleFor(a => a.QuestionId)
                        .GreaterThan(0).WithMessage("QuestionId is required.");

                    answer.RuleFor(a => a)
                        .Must(a => a.OptionId.HasValue || !string.IsNullOrWhiteSpace(a.AnswerText))
                        .WithMessage("Either OptionId or AnswerText must be provided.");
                });
        }
    }
}
