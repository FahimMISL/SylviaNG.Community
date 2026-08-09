using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyQuestionGetAll
{
    public class SurveyQuestionGetAllQuery : IRequest<List<SurveyQuestionResponse>>
    {
        public long SurveyId { get; set; }

        public SurveyQuestionGetAllQuery(long surveyId)
        {
            SurveyId = surveyId;
        }
    }
}
