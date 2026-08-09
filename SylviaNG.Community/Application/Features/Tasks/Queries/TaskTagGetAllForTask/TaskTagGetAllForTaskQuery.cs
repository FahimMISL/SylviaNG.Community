using MediatR;
using SylviaNG.Community.Application.Features.TaskTags.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskTagGetAllForTask
{
    public class TaskTagGetAllForTaskQuery : IRequest<List<TaskTagResponse>>
    {
        public long TaskId { get; set; }

        public TaskTagGetAllForTaskQuery(long taskId)
        {
            TaskId = taskId;
        }
    }
}
