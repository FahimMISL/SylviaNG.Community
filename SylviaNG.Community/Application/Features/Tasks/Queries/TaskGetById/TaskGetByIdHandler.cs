using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetById
{
    public class TaskGetByIdHandler : IRequestHandler<TaskGetByIdQuery, TaskResponse>
    {
        private readonly ITaskService _taskService;

        public TaskGetByIdHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<TaskResponse> Handle(TaskGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _taskService.GetByIdAsync(query.TaskId);
        }
    }
}
