using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionCandidateGetAll
{
    public class ElectionCandidateGetAllQuery : IRequest<List<ElectionCandidateResponse>>
    {
        public long ElectionId { get; set; }

        public ElectionCandidateGetAllQuery(long electionId)
        {
            ElectionId = electionId;
        }
    }
}
