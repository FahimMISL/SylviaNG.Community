using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskBulkReassign
{
    public class TaskBulkReassignHandler : IRequestHandler<TaskBulkReassignCommand>
    {
        private readonly ITaskService _taskService;

        public TaskBulkReassignHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task Handle(TaskBulkReassignCommand command, CancellationToken cancellationToken)
        {
            await _taskService.BulkReassignAsync(
                command.Request.TaskIds,
                command.Request.NewAssignedTo,
                command.CallerEmployeeId,
                command.IsHrOrAdmin);
        }
    }
}
