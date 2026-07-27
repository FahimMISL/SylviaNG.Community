using MediatR;
using SylviaNG.Community.Application.Features.ActivityLogs.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ActivityLogs.Queries.ActivityLogGetAllPaged
{
    public class ActivityLogGetAllPagedQuery : IRequest<PagedResult<ActivityLogResponse>>
    {
        public PagedRequest Request { get; set; }

        public ActivityLogGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
