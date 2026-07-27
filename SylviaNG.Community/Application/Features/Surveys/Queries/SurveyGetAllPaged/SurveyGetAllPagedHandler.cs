using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetAllPaged
{
    public class SurveyGetAllPagedHandler : IRequestHandler<SurveyGetAllPagedQuery, PagedResult<SurveyDetailResponse>>
    {
        private readonly ISurveyService _surveyService;

        public SurveyGetAllPagedHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<PagedResult<SurveyDetailResponse>> Handle(SurveyGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _surveyService.GetPaginatedAsync(query.Request);
        }
    }
}
