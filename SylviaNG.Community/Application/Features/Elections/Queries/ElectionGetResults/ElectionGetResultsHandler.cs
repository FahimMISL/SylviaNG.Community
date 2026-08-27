using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetResults
{
    public class ElectionGetResultsHandler : IRequestHandler<ElectionGetResultsQuery, ElectionResultsResponse>
    {
        private readonly IElectionService _electionService;

        public ElectionGetResultsHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<ElectionResultsResponse> Handle(ElectionGetResultsQuery query, CancellationToken cancellationToken)
        {
            return await _electionService.GetResultsAsync(query.ElectionId);
        }
    }
}
