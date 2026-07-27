using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Polls.Commands.PollVoteCast
{
    public class PollVoteCastHandler : IRequestHandler<PollVoteCastCommand, long>
    {
        private readonly IPollService _pollService;

        public PollVoteCastHandler(IPollService pollService)
        {
            _pollService = pollService;
        }

        public async Task<long> Handle(PollVoteCastCommand command, CancellationToken cancellationToken)
        {
            return await _pollService.VoteAsync(command.PostId, command.Request);
        }
    }
}
