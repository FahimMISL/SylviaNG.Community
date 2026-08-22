using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskBulkCancel
{
    public class TaskBulkCancelHandler : IRequestHandler<TaskBulkCancelCommand>
    {
        private readonly ITaskService _taskService;

        public TaskBulkCancelHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task Handle(TaskBulkCancelCommand command, CancellationToken cancellationToken)
        {
            await _taskService.BulkCancelAsync(command.Request.TaskIds, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
