using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskCreate
{
    public class TaskCreateCommand : IRequest<long>
    {
        public TaskCreateRequest Request { get; set; }
        public long? CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TaskCreateCommand(TaskCreateRequest request, long? callerEmployeeId, bool isHrOrAdmin)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
