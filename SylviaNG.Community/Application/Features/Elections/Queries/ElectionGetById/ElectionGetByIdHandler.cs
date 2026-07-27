using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetById
{
    public class ElectionGetByIdHandler : IRequestHandler<ElectionGetByIdQuery, ElectionResponse>
    {
        private readonly IElectionService _electionService;

        public ElectionGetByIdHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<ElectionResponse> Handle(ElectionGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _electionService.GetByIdAsync(query.ElectionId);
        }
    }
}
