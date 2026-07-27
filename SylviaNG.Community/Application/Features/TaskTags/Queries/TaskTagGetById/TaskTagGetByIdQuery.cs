using MediatR;
using SylviaNG.Community.Application.Features.TaskTags.Models;

namespace SylviaNG.Community.Application.Features.TaskTags.Queries.TaskTagGetById
{
    public class TaskTagGetByIdQuery : IRequest<TaskTagResponse>
    {
        public long TagId { get; set; }

        public TaskTagGetByIdQuery(long tagId)
        {
            TagId = tagId;
        }
    }
}
