using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetAllPaged
{
    public class TaskGetAllPagedQuery : IRequest<PagedResult<TaskResponse>>
    {
        public TaskFilterRequest Request { get; set; }

        public TaskGetAllPagedQuery(TaskFilterRequest request)
        {
            Request = request;
        }
    }
}
