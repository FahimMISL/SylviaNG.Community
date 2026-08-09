using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyAudienceGetAll
{
    public class SurveyAudienceGetAllQuery : IRequest<List<SurveyAudienceResponse>>
    {
        public long SurveyId { get; set; }

        public SurveyAudienceGetAllQuery(long surveyId)
        {
            SurveyId = surveyId;
        }
    }
}
