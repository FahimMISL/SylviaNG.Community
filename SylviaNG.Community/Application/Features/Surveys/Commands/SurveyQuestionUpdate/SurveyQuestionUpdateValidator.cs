using FluentValidation;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionUpdate
{
    public class SurveyQuestionUpdateValidator : AbstractValidator<SurveyQuestionUpdateCommand>
    {
        public SurveyQuestionUpdateValidator()
        {
            RuleFor(x => x.Request.QuestionType)
                .MaximumLength(50).WithMessage("QuestionType must not exceed 50 characters.")
                .When(x => x.Request.QuestionType != null);
        }
    }
}
