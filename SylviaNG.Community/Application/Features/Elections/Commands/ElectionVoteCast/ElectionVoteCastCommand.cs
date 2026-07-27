using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionVoteCast
{
    public class ElectionVoteCastCommand : IRequest<long>
    {
        public long ElectionId { get; set; }
        public ElectionVoteCastRequest Request { get; set; }

        /// <summary>
        /// Resolved server-side from ICurrentUserService by the controller - never trusted
        /// from the request body, to prevent a caller from voting on someone else's behalf.
        /// </summary>
        public long VoterId { get; set; }

        public ElectionVoteCastCommand(long electionId, ElectionVoteCastRequest request, long voterId)
        {
            ElectionId = electionId;
            Request = request;
            VoterId = voterId;
        }
    }
}
