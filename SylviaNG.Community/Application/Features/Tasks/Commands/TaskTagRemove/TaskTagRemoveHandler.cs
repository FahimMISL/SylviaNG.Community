using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskTagRemove
{
    public class TaskTagRemoveHandler : IRequestHandler<TaskTagRemoveCommand>
    {
        private readonly ITaskService _taskService;

        public TaskTagRemoveHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task Handle(TaskTagRemoveCommand command, CancellationToken cancellationToken)
        {
            await _taskService.RemoveTagAsync(command.TaskId, command.TagId);
        }
    }
}
