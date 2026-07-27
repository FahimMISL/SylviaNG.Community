using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskTagAssign
{
    public class TaskTagAssignHandler : IRequestHandler<TaskTagAssignCommand, long>
    {
        private readonly ITaskService _taskService;

        public TaskTagAssignHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<long> Handle(TaskTagAssignCommand command, CancellationToken cancellationToken)
        {
            return await _taskService.AssignTagAsync(command.TaskId, command.Request);
        }
    }
}
