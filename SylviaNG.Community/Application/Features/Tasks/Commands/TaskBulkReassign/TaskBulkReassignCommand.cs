using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskBulkReassign
{
    public class TaskBulkReassignCommand : IRequest
    {
        public TaskBulkReassignRequest Request { get; set; }
        public long? CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TaskBulkReassignCommand(TaskBulkReassignRequest request, long? callerEmployeeId, bool isHrOrAdmin)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
