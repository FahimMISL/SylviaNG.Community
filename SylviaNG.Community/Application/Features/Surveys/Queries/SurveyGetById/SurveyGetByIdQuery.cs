using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetById
{
    public class SurveyGetByIdQuery : IRequest<SurveyDetailResponse>
    {
        public long SurveyId { get; set; }

        public SurveyGetByIdQuery(long surveyId)
        {
            SurveyId = surveyId;
        }
    }
}
