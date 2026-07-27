using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionUpdate
{
    public class SurveyQuestionUpdateCommand : IRequest
    {
        public long SurveyId { get; set; }
        public long QuestionId { get; set; }
        public SurveyQuestionUpdateRequest Request { get; set; }

        public SurveyQuestionUpdateCommand(long surveyId, long questionId, SurveyQuestionUpdateRequest request)
        {
            SurveyId = surveyId;
            QuestionId = questionId;
            Request = request;
        }
    }
}
