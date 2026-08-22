using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetAssignedByMePaged
{
    /// <summary>US-7.7: individual (non-team) tasks the caller assigned to others. AssignedBy/TeamId
    /// are forced server-side in the handler.</summary>
    public class TaskGetAssignedByMePagedQuery : IRequest<PagedResult<TaskResponse>>
    {
        public TaskFilterRequest Request { get; set; }
        public long? CallerEmployeeId { get; set; }

        public TaskGetAssignedByMePagedQuery(TaskFilterRequest request, long? callerEmployeeId)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
