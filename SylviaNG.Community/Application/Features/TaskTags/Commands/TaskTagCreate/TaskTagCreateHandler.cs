using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.TaskTags.Commands.TaskTagCreate
{
    public class TaskTagCreateHandler : IRequestHandler<TaskTagCreateCommand, long>
    {
        private readonly ITaskTagService _taskTagService;

        public TaskTagCreateHandler(ITaskTagService taskTagService)
        {
            _taskTagService = taskTagService;
        }

        public async Task<long> Handle(TaskTagCreateCommand command, CancellationToken cancellationToken)
        {
            return await _taskTagService.CreateAsync(command.Request);
        }
    }
}
