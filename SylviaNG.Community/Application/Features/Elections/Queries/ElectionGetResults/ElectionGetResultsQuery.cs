using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetResults
{
    public class ElectionGetResultsQuery : IRequest<ElectionResultsResponse>
    {
        public long ElectionId { get; set; }

        public ElectionGetResultsQuery(long electionId)
        {
            ElectionId = electionId;
        }
    }
}
