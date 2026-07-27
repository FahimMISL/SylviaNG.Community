using MediatR;
using SylviaNG.Community.Application.Features.Polls.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Polls.Queries.PollGetByPostId
{
    public class PollGetByPostIdHandler : IRequestHandler<PollGetByPostIdQuery, PollResponse>
    {
        private readonly IPollService _pollService;

        public PollGetByPostIdHandler(IPollService pollService)
        {
            _pollService = pollService;
        }

        public async Task<PollResponse> Handle(PollGetByPostIdQuery query, CancellationToken cancellationToken)
        {
            return await _pollService.GetByPostIdAsync(query.PostId);
        }
    }
}
