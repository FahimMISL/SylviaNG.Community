using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Polls.Commands.PollCreate
{
    public class PollCreateHandler : IRequestHandler<PollCreateCommand, long>
    {
        private readonly IPollService _pollService;

        public PollCreateHandler(IPollService pollService)
        {
            _pollService = pollService;
        }

        public async Task<long> Handle(PollCreateCommand command, CancellationToken cancellationToken)
        {
            return await _pollService.CreateAsync(command.PostId, command.Request);
        }
    }
}
