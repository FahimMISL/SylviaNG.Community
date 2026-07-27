using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCreate
{
    public class ElectionCreateHandler : IRequestHandler<ElectionCreateCommand, long>
    {
        private readonly IElectionService _electionService;

        public ElectionCreateHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<long> Handle(ElectionCreateCommand command, CancellationToken cancellationToken)
        {
            return await _electionService.CreateAsync(command.Request, command.CreatedBy);
        }
    }
}
