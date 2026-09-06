using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetAllPaged
{
    public class SurveyGetAllPagedQuery : IRequest<PagedResult<SurveyDetailResponse>>
    {
        public PagedRequest Request { get; set; }

        /// <summary>Resolved server-side from ICurrentUserService by SurveyController, used to compute
        /// SurveyDetailResponse.IsEligible per survey - see SurveyService.GetPaginatedAsync.</summary>
        public long? EmployeeId { get; set; }

        public SurveyGetAllPagedQuery(PagedRequest request, long? employeeId)
        {
            Request = request;
            EmployeeId = employeeId;
        }
    }
}
