using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskDelete
{
    public class TaskDeleteHandler : IRequestHandler<TaskDeleteCommand>
    {
        private readonly ITaskService _taskService;

        public TaskDeleteHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task Handle(TaskDeleteCommand command, CancellationToken cancellationToken)
        {
            await _taskService.DeleteAsync(command.TaskId);
        }
    }
}
