using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionCandidateGetAll
{
    public class ElectionCandidateGetAllHandler : IRequestHandler<ElectionCandidateGetAllQuery, List<ElectionCandidateResponse>>
    {
        private readonly IElectionService _electionService;

        public ElectionCandidateGetAllHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<List<ElectionCandidateResponse>> Handle(ElectionCandidateGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _electionService.GetCandidatesAsync(query.ElectionId);
        }
    }
}
