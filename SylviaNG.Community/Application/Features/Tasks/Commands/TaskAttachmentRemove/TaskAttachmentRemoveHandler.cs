using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskAttachmentRemove
{
    public class TaskAttachmentRemoveHandler : IRequestHandler<TaskAttachmentRemoveCommand>
    {
        private readonly ITaskService _taskService;

        public TaskAttachmentRemoveHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task Handle(TaskAttachmentRemoveCommand command, CancellationToken cancellationToken)
        {
            await _taskService.RemoveAttachmentAsync(command.TaskId, command.AttachmentId, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
