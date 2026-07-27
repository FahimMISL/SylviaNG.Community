using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionAudienceTargetGetAll
{
    public class ElectionAudienceTargetGetAllQuery : IRequest<List<ElectionAudienceTargetResponse>>
    {
        public long ElectionId { get; set; }

        public ElectionAudienceTargetGetAllQuery(long electionId)
        {
            ElectionId = electionId;
        }
    }
}
