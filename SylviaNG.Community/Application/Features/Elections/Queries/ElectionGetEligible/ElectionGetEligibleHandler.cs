using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetEligible
{
    public class ElectionGetEligibleHandler : IRequestHandler<ElectionGetEligibleQuery, List<ElectionEligibleResponse>>
    {
        private readonly IElectionService _electionService;

        public ElectionGetEligibleHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<List<ElectionEligibleResponse>> Handle(ElectionGetEligibleQuery query, CancellationToken cancellationToken)
        {
            return await _electionService.GetEligibleAsync(query.EmployeeId);
        }
    }
}
