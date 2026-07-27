using MediatR;
using SylviaNG.Community.Application.Features.Polls.Models;

namespace SylviaNG.Community.Application.Features.Polls.Queries.PollGetByPostId
{
    public class PollGetByPostIdQuery : IRequest<PollResponse>
    {
        public long PostId { get; set; }

        public PollGetByPostIdQuery(long postId)
        {
            PostId = postId;
        }
    }
}
