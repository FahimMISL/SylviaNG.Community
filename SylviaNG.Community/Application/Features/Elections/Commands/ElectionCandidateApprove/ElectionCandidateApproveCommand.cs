using MediatR;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateApprove
{
    public class ElectionCandidateApproveCommand : IRequest
    {
        public long ElectionId { get; set; }
        public long CandidateId { get; set; }

        public ElectionCandidateApproveCommand(long electionId, long candidateId)
        {
            ElectionId = electionId;
            CandidateId = candidateId;
        }
    }
}
