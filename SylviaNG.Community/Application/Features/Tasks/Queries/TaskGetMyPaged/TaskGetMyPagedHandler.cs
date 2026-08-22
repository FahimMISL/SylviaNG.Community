using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetMyPaged
{
    public class TaskGetMyPagedHandler : IRequestHandler<TaskGetMyPagedQuery, PagedResult<TaskResponse>>
    {
        private readonly ITaskService _taskService;

        public TaskGetMyPagedHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<PagedResult<TaskResponse>> Handle(TaskGetMyPagedQuery query, CancellationToken cancellationToken)
        {
            return await _taskService.GetMyPaginatedAsync(query.Request, query.CallerEmployeeId);
        }
    }
}
