using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskHistoryGetAll
{
    public class TaskHistoryGetAllHandler : IRequestHandler<TaskHistoryGetAllQuery, List<TaskHistoryResponse>>
    {
        private readonly ITaskService _taskService;

        public TaskHistoryGetAllHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<List<TaskHistoryResponse>> Handle(TaskHistoryGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _taskService.GetHistoryAsync(query.TaskId);
        }
    }
}
