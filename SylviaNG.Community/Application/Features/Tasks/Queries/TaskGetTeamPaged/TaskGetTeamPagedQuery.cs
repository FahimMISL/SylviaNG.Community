using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetTeamPaged
{
    /// <summary>US-7.5: a team's task board. Caller must be that team's Supervisor, or HR/Admin -
    /// enforced in the handler, TeamId is forced server-side.</summary>
    public class TaskGetTeamPagedQuery : IRequest<PagedResult<TaskResponse>>
    {
        public long TeamId { get; set; }
        public TaskFilterRequest Request { get; set; }
        public long? CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TaskGetTeamPagedQuery(long teamId, TaskFilterRequest request, long? callerEmployeeId, bool isHrOrAdmin)
        {
            TeamId = teamId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
