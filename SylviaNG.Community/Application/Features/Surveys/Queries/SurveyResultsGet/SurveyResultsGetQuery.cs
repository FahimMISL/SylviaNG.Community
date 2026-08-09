using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyResultsGet
{
    public class SurveyResultsGetQuery : IRequest<SurveyResultsResponse>
    {
        public long SurveyId { get; set; }

        public SurveyResultsGetQuery(long surveyId)
        {
            SurveyId = surveyId;
        }
    }
}
