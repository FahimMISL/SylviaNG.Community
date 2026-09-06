using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateNominateBulk
{
    public class ElectionCandidateNominateBulkCommand : IRequest<int>
    {
        public long ElectionId { get; set; }
        public ElectionCandidateNominateBulkRequest Request { get; set; }

        public ElectionCandidateNominateBulkCommand(long electionId, ElectionCandidateNominateBulkRequest request)
        {
            ElectionId = electionId;
            Request = request;
        }
    }
}
