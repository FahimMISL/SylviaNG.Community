using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskCreate
{
    public class TaskCreateHandler : IRequestHandler<TaskCreateCommand, long>
    {
        private readonly ITaskService _taskService;

        public TaskCreateHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<long> Handle(TaskCreateCommand command, CancellationToken cancellationToken)
        {
            return await _taskService.CreateAsync(command.Request, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
