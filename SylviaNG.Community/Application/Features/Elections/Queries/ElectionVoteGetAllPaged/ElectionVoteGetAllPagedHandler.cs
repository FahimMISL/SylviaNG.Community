using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionVoteGetAllPaged
{
    public class ElectionVoteGetAllPagedHandler : IRequestHandler<ElectionVoteGetAllPagedQuery, PagedResult<ElectionVoteResponse>>
    {
        private readonly IElectionService _electionService;

        public ElectionVoteGetAllPagedHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<PagedResult<ElectionVoteResponse>> Handle(ElectionVoteGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _electionService.GetVotesPaginatedAsync(query.ElectionId, query.Request);
        }
    }
}
