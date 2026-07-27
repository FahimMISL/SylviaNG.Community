using MediatR;
using SylviaNG.Community.Application.Features.Polls.Models;

namespace SylviaNG.Community.Application.Features.Polls.Commands.PollCreate
{
    public class PollCreateCommand : IRequest<long>
    {
        public long PostId { get; set; }
        public PollCreateRequest Request { get; set; }

        public PollCreateCommand(long postId, PollCreateRequest request)
        {
            PostId = postId;
            Request = request;
        }
    }
}
