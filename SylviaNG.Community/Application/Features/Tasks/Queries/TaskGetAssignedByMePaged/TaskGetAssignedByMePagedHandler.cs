using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetAssignedByMePaged
{
    public class TaskGetAssignedByMePagedHandler : IRequestHandler<TaskGetAssignedByMePagedQuery, PagedResult<TaskResponse>>
    {
        private readonly ITaskService _taskService;

        public TaskGetAssignedByMePagedHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<PagedResult<TaskResponse>> Handle(TaskGetAssignedByMePagedQuery query, CancellationToken cancellationToken)
        {
            return await _taskService.GetAssignedByMePaginatedAsync(query.Request, query.CallerEmployeeId);
        }
    }
}
