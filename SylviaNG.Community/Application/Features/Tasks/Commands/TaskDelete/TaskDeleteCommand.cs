using MediatR;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskDelete
{
    public class TaskDeleteCommand : IRequest
    {
        public long TaskId { get; set; }
        public long? CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TaskDeleteCommand(long taskId, long? callerEmployeeId, bool isHrOrAdmin)
        {
            TaskId = taskId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
