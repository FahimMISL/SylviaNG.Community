using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetAllPaged
{
    public class ElectionGetAllPagedHandler : IRequestHandler<ElectionGetAllPagedQuery, PagedResult<ElectionResponse>>
    {
        private readonly IElectionService _electionService;

        public ElectionGetAllPagedHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<PagedResult<ElectionResponse>> Handle(ElectionGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _electionService.GetPaginatedAsync(query.Request);
        }
    }
}
