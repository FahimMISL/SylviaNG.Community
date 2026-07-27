using MediatR;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskTagGetAllForTask
{
    public class TaskTagGetAllForTaskHandler : IRequestHandler<TaskTagGetAllForTaskQuery, List<TaskTagResponse>>
    {
        private readonly ITaskService _taskService;

        public TaskTagGetAllForTaskHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<List<TaskTagResponse>> Handle(TaskTagGetAllForTaskQuery query, CancellationToken cancellationToken)
        {
            return await _taskService.GetTagsAsync(query.TaskId);
        }
    }
}
