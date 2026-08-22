using MediatR;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskAttachmentRemove
{
    public class TaskAttachmentRemoveCommand : IRequest
    {
        public long TaskId { get; set; }
        public long AttachmentId { get; set; }
        public long? CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TaskAttachmentRemoveCommand(long taskId, long attachmentId, long? callerEmployeeId, bool isHrOrAdmin)
        {
            TaskId = taskId;
            AttachmentId = attachmentId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
