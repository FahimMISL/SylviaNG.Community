using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskCommentGetAll
{
    public class TaskCommentGetAllQuery : IRequest<List<TaskCommentResponse>>
    {
        public long TaskId { get; set; }

        public TaskCommentGetAllQuery(long taskId)
        {
            TaskId = taskId;
        }
    }
}
