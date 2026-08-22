using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetMyPaged
{
    /// <summary>US-7.8: tasks assigned to the caller. AssignedTo is forced server-side in the handler.</summary>
    public class TaskGetMyPagedQuery : IRequest<PagedResult<TaskResponse>>
    {
        public TaskFilterRequest Request { get; set; }
        public long? CallerEmployeeId { get; set; }

        public TaskGetMyPagedQuery(TaskFilterRequest request, long? callerEmployeeId)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
