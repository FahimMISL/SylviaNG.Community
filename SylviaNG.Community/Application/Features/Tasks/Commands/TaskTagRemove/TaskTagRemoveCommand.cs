using MediatR;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskTagRemove
{
    public class TaskTagRemoveCommand : IRequest
    {
        public long TaskId { get; set; }
        public long TagId { get; set; }

        public TaskTagRemoveCommand(long taskId, long tagId)
        {
            TaskId = taskId;
            TagId = tagId;
        }
    }
}
