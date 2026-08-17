using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyResultsGet
{
    public class SurveyResultsGetHandler : IRequestHandler<SurveyResultsGetQuery, SurveyResultsResponse>
    {
        private readonly ISurveyService _surveyService;

        public SurveyResultsGetHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<SurveyResultsResponse> Handle(SurveyResultsGetQuery query, CancellationToken cancellationToken)
        {
            return await _surveyService.GetResultsAsync(query.SurveyId);
        }
    }
}
