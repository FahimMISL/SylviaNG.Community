using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyResponseGetAllPaged
{
    public class SurveyResponseGetAllPagedQuery : IRequest<PagedResult<SurveySubmissionResponse>>
    {
        public long SurveyId { get; set; }
        public PagedRequest Request { get; set; }

        public SurveyResponseGetAllPagedQuery(long surveyId, PagedRequest request)
        {
            SurveyId = surveyId;
            Request = request;
        }
    }
}
