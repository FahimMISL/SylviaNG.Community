using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskCommentAdd
{
    public class TaskCommentAddCommand : IRequest<long>
    {
        public long TaskId { get; set; }
        public TaskCommentAddRequest Request { get; set; }
        public long? CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TaskCommentAddCommand(long taskId, TaskCommentAddRequest request, long? callerEmployeeId, bool isHrOrAdmin)
        {
            TaskId = taskId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
