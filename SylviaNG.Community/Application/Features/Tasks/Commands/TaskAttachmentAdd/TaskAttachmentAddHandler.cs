using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskAttachmentAdd
{
    public class TaskAttachmentAddHandler : IRequestHandler<TaskAttachmentAddCommand, long>
    {
        private readonly ITaskService _taskService;

        public TaskAttachmentAddHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<long> Handle(TaskAttachmentAddCommand command, CancellationToken cancellationToken)
        {
            return await _taskService.AddAttachmentAsync(command.TaskId, command.Request);
        }
    }
}
