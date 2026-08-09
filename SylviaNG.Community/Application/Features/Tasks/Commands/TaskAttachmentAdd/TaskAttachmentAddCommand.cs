using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskAttachmentAdd
{
    public class TaskAttachmentAddCommand : IRequest<long>
    {
        public long TaskId { get; set; }
        public TaskAttachmentAddRequest Request { get; set; }

        public TaskAttachmentAddCommand(long taskId, TaskAttachmentAddRequest request)
        {
            TaskId = taskId;
            Request = request;
        }
    }
}
