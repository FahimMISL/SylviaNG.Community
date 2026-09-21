using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateUpdateManifesto
{
    public class ElectionCandidateUpdateManifestoCommand : IRequest
    {
        public long ElectionId { get; set; }
        public long CandidateId { get; set; }
        public ElectionCandidateUpdateManifestoRequest Request { get; set; }

        public ElectionCandidateUpdateManifestoCommand(long electionId, long candidateId, ElectionCandidateUpdateManifestoRequest request)
        {
            ElectionId = electionId;
            CandidateId = candidateId;
            Request = request;
        }
    }
}
