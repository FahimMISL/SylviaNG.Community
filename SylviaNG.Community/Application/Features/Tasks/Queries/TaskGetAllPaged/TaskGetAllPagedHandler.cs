using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetAllPaged
{
    public class TaskGetAllPagedHandler : IRequestHandler<TaskGetAllPagedQuery, PagedResult<TaskResponse>>
    {
        private readonly ITaskService _taskService;

        public TaskGetAllPagedHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<PagedResult<TaskResponse>> Handle(TaskGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _taskService.GetPaginatedAsync(query.Request);
        }
    }
}
