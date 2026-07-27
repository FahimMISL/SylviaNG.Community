using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskCommentAdd
{
    public class TaskCommentAddHandler : IRequestHandler<TaskCommentAddCommand, long>
    {
        private readonly ITaskService _taskService;

        public TaskCommentAddHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<long> Handle(TaskCommentAddCommand command, CancellationToken cancellationToken)
        {
            return await _taskService.AddCommentAsync(command.TaskId, command.Request);
        }
    }
}
