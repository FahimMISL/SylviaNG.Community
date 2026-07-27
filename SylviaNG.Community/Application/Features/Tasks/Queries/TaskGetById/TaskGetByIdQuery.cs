using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetById
{
    public class TaskGetByIdQuery : IRequest<TaskResponse>
    {
        public long TaskId { get; set; }

        public TaskGetByIdQuery(long taskId)
        {
            TaskId = taskId;
        }
    }
}
