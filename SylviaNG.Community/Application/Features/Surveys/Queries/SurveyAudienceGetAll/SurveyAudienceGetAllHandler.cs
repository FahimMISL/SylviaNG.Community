using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyAudienceGetAll
{
    public class SurveyAudienceGetAllHandler : IRequestHandler<SurveyAudienceGetAllQuery, List<SurveyAudienceResponse>>
    {
        private readonly ISurveyService _surveyService;

        public SurveyAudienceGetAllHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<List<SurveyAudienceResponse>> Handle(SurveyAudienceGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _surveyService.GetAudienceAsync(query.SurveyId);
        }
    }
}
