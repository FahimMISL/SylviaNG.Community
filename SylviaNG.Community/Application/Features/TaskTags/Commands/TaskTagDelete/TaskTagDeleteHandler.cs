using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.TaskTags.Commands.TaskTagDelete
{
    public class TaskTagDeleteHandler : IRequestHandler<TaskTagDeleteCommand>
    {
        private readonly ITaskTagService _taskTagService;

        public TaskTagDeleteHandler(ITaskTagService taskTagService)
        {
            _taskTagService = taskTagService;
        }

        public async Task Handle(TaskTagDeleteCommand command, CancellationToken cancellationToken)
        {
            await _taskTagService.DeleteAsync(command.TagId);
        }
    }
}
