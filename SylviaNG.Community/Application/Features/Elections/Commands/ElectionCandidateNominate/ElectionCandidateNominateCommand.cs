using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateNominate
{
    public class ElectionCandidateNominateCommand : IRequest<long>
    {
        public long ElectionId { get; set; }
        public ElectionCandidateNominateRequest Request { get; set; }

        public ElectionCandidateNominateCommand(long electionId, ElectionCandidateNominateRequest request)
        {
            ElectionId = electionId;
            Request = request;
        }
    }
}
