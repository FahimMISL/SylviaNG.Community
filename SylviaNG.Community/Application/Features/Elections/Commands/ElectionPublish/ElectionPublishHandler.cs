using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionPublish
{
    public class ElectionPublishHandler : IRequestHandler<ElectionPublishCommand>
    {
        private readonly IElectionService _electionService;

        public ElectionPublishHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task Handle(ElectionPublishCommand command, CancellationToken cancellationToken)
        {
            await _electionService.PublishAsync(command.ElectionId);
        }
    }
}
