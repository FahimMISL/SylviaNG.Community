using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyResponseGetAllPaged
{
    public class SurveyResponseGetAllPagedHandler : IRequestHandler<SurveyResponseGetAllPagedQuery, PagedResult<SurveySubmissionResponse>>
    {
        private readonly ISurveyService _surveyService;

        public SurveyResponseGetAllPagedHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<PagedResult<SurveySubmissionResponse>> Handle(SurveyResponseGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _surveyService.GetResponsesAsync(query.SurveyId, query.Request);
        }
    }
}
