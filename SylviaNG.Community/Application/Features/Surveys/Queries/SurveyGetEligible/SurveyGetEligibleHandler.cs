using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetEligible
{
    public class SurveyGetEligibleHandler : IRequestHandler<SurveyGetEligibleQuery, List<SurveyDetailResponse>>
    {
        private readonly ISurveyService _surveyService;

        public SurveyGetEligibleHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<List<SurveyDetailResponse>> Handle(SurveyGetEligibleQuery query, CancellationToken cancellationToken)
        {
            return await _surveyService.GetEligibleAsync(query.EmployeeId);
        }
    }
}
