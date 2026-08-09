using FluentValidation;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyResponseSubmit
{
    public class SurveyResponseSubmitValidator : AbstractValidator<SurveyResponseSubmitCommand>
    {
        public SurveyResponseSubmitValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            // Answers may legitimately be empty: external surveys (Survey.ExternalUrl set) have no
            // CES-native SurveyQuestion rows at all, so "taking" one submits a response with zero
            // answers purely to record completion - SurveyService.SubmitResponseAsync already
            // handles this correctly (it only touches SurveyAnswer rows when Answers.Count > 0).

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
