using MediatR;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskAttachmentRemove
{
    public class TaskAttachmentRemoveCommand : IRequest
    {
        public long TaskId { get; set; }
        public long AttachmentId { get; set; }

        public TaskAttachmentRemoveCommand(long taskId, long attachmentId)
        {
            TaskId = taskId;
            AttachmentId = attachmentId;
        }
    }
}
