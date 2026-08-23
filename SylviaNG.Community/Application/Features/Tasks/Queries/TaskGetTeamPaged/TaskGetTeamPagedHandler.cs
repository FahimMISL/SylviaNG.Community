using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetTeamPaged
{
    public class TaskGetTeamPagedHandler : IRequestHandler<TaskGetTeamPagedQuery, PagedResult<TaskResponse>>
    {
        private readonly ITaskService _taskService;

        public TaskGetTeamPagedHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<PagedResult<TaskResponse>> Handle(TaskGetTeamPagedQuery query, CancellationToken cancellationToken)
        {
            return await _taskService.GetTeamPaginatedAsync(query.TeamId, query.Request, query.CallerEmployeeId, query.IsHrOrAdmin);
        }
    }
}
