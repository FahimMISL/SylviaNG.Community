using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskUpdate
{
    public class TaskUpdateCommand : IRequest
    {
        public long TaskId { get; set; }
        public TaskUpdateRequest Request { get; set; }

        /// <summary>
        /// Resolved from ICurrentUserService.EmployeeId by the controller - recorded as
        /// TaskHistory.ChangedBy when this update triggers a history entry.
        /// </summary>
        public long? ChangedBy { get; set; }

        public TaskUpdateCommand(long taskId, TaskUpdateRequest request, long? changedBy)
        {
            TaskId = taskId;
            Request = request;
            ChangedBy = changedBy;
        }
    }
}
