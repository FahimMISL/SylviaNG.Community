using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetAllPaged
{
    public class SurveyGetAllPagedQuery : IRequest<PagedResult<SurveyDetailResponse>>
    {
        public PagedRequest Request { get; set; }

        public SurveyGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
