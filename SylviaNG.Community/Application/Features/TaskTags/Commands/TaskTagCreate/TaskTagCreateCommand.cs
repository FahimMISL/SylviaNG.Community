using MediatR;
using SylviaNG.Community.Application.Features.TaskTags.Models;

namespace SylviaNG.Community.Application.Features.TaskTags.Commands.TaskTagCreate
{
    public class TaskTagCreateCommand : IRequest<long>
    {
        public TaskTagCreateRequest Request { get; set; }

        public TaskTagCreateCommand(TaskTagCreateRequest request)
        {
            Request = request;
        }
    }
}
