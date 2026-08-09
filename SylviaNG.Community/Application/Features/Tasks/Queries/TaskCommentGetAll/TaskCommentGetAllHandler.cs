using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskCommentGetAll
{
    public class TaskCommentGetAllHandler : IRequestHandler<TaskCommentGetAllQuery, List<TaskCommentResponse>>
    {
        private readonly ITaskService _taskService;

        public TaskCommentGetAllHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<List<TaskCommentResponse>> Handle(TaskCommentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _taskService.GetCommentsAsync(query.TaskId);
        }
    }
}
