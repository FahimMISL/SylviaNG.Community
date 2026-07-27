using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskAttachmentGetAll
{
    public class TaskAttachmentGetAllQuery : IRequest<List<TaskAttachmentResponse>>
    {
        public long TaskId { get; set; }

        public TaskAttachmentGetAllQuery(long taskId)
        {
            TaskId = taskId;
        }
    }
}
