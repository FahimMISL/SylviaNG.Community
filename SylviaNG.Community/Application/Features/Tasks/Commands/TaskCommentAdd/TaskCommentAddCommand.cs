using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskCommentAdd
{
    public class TaskCommentAddCommand : IRequest<long>
    {
        public long TaskId { get; set; }
        public TaskCommentAddRequest Request { get; set; }

        public TaskCommentAddCommand(long taskId, TaskCommentAddRequest request)
        {
            TaskId = taskId;
            Request = request;
        }
    }
}
