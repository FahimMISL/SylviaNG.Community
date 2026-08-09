using MediatR;
using SylviaNG.Community.Application.Features.Polls.Models;

namespace SylviaNG.Community.Application.Features.Polls.Commands.PollVoteCast
{
    public class PollVoteCastCommand : IRequest<long>
    {
        public long PostId { get; set; }
        public PollVoteRequest Request { get; set; }

        public PollVoteCastCommand(long postId, PollVoteRequest request)
        {
            PostId = postId;
            Request = request;
        }
    }
}
