using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskUpdate
{
    public class TaskUpdateHandler : IRequestHandler<TaskUpdateCommand>
    {
        private readonly ITaskService _taskService;

        public TaskUpdateHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task Handle(TaskUpdateCommand command, CancellationToken cancellationToken)
        {
            await _taskService.UpdateAsync(command.TaskId, command.Request, command.ChangedBy, command.IsHrOrAdmin);
        }
    }
}
