using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionAudienceTargetGetAll
{
    public class ElectionAudienceTargetGetAllHandler : IRequestHandler<ElectionAudienceTargetGetAllQuery, List<ElectionAudienceTargetResponse>>
    {
        private readonly IElectionService _electionService;

        public ElectionAudienceTargetGetAllHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<List<ElectionAudienceTargetResponse>> Handle(ElectionAudienceTargetGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _electionService.GetAudienceTargetsAsync(query.ElectionId);
        }
    }
}
