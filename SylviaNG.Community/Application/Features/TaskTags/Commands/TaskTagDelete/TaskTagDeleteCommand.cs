using MediatR;

namespace SylviaNG.Community.Application.Features.TaskTags.Commands.TaskTagDelete
{
    public class TaskTagDeleteCommand : IRequest
    {
        public long TagId { get; set; }

        public TaskTagDeleteCommand(long tagId)
        {
            TagId = tagId;
        }
    }
}
