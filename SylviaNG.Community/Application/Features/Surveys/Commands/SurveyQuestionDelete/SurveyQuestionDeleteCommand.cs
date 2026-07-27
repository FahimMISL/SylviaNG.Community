using MediatR;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionDelete
{
    public class SurveyQuestionDeleteCommand : IRequest
    {
        public long SurveyId { get; set; }
        public long QuestionId { get; set; }

        public SurveyQuestionDeleteCommand(long surveyId, long questionId)
        {
            SurveyId = surveyId;
            QuestionId = questionId;
        }
    }
}
