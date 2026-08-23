using FluentValidation;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionUpdate
{
    public class SurveyQuestionUpdateValidator : AbstractValidator<SurveyQuestionUpdateCommand>
    {
        public SurveyQuestionUpdateValidator()
        {
            RuleFor(x => x.Request.QuestionType)
                .Must(t => SurveyQuestionTypes.All.Contains(t))
                .WithMessage($"QuestionType must be one of: {string.Join(", ", SurveyQuestionTypes.All)}.")
                .When(x => x.Request.QuestionType != null);
        }
    }
}
