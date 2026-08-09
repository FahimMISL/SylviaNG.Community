using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Commands.TaskTagAssign
{
    public class TaskTagAssignCommand : IRequest<long>
    {
        public long TaskId { get; set; }
        public TaskTagAssignRequest Request { get; set; }

        public TaskTagAssignCommand(long taskId, TaskTagAssignRequest request)
        {
            TaskId = taskId;
            Request = request;
        }
    }
}
