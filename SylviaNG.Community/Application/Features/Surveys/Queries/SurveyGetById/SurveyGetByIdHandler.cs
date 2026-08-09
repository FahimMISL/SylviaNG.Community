using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetById
{
    public class SurveyGetByIdHandler : IRequestHandler<SurveyGetByIdQuery, SurveyDetailResponse>
    {
        private readonly ISurveyService _surveyService;

        public SurveyGetByIdHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<SurveyDetailResponse> Handle(SurveyGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _surveyService.GetByIdAsync(query.SurveyId);
        }
    }
}
