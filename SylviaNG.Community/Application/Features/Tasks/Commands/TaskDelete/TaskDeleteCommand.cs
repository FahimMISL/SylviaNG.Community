using MediatR;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskDelete
{
    public class TaskDeleteCommand : IRequest
    {
        public long TaskId { get; set; }

        public TaskDeleteCommand(long taskId)
        {
            TaskId = taskId;
        }
    }
}
