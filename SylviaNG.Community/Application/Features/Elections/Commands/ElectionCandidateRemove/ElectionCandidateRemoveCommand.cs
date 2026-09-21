using MediatR;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateRemove
{
    public class ElectionCandidateRemoveCommand : IRequest
    {
        public long ElectionId { get; set; }
        public long CandidateId { get; set; }

        public ElectionCandidateRemoveCommand(long electionId, long candidateId)
        {
            ElectionId = electionId;
            CandidateId = candidateId;
        }
    }
}
