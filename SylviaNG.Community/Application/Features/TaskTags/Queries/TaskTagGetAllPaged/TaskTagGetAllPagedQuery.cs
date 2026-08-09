using MediatR;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.TaskTags.Queries.TaskTagGetAllPaged
{
    public class TaskTagGetAllPagedQuery : IRequest<PagedResult<TaskTagResponse>>
    {
        public PagedRequest Request { get; set; }

        public TaskTagGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
