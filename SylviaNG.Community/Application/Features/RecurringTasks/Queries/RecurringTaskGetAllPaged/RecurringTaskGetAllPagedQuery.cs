using MediatR;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Queries.RecurringTaskGetAllPaged
{
    public class RecurringTaskGetAllPagedQuery : IRequest<PagedResult<RecurringTaskResponse>>
    {
        public PagedRequest Request { get; set; }

        public RecurringTaskGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
