using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskBulkCancel
{
    public class TaskBulkCancelCommand : IRequest
    {
        public TaskBulkCancelRequest Request { get; set; }
        public long? CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TaskBulkCancelCommand(TaskBulkCancelRequest request, long? callerEmployeeId, bool isHrOrAdmin)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
