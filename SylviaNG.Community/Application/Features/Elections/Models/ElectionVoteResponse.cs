namespace SylviaNG.Community.Application.Features.Elections.Models
{
    public class ElectionVoteResponse
    {
        public long ElectionVoteId { get; set; }
        public long ElectionId { get; set; }
        public long CandidateId { get; set; }

        /// <summary>
        /// Null when the owning Election.IsAnonymous is true (see ElectionMapper.ToResponse) -
        /// tradeoff: the GET votes endpoint is left open to any authenticated employee rather
        /// than HRAdminOnly, since hiding VoterId here already protects anonymity for the
        /// common case; an HRAdminOnly-only endpoint would be stricter but was judged
        /// unnecessary extra friction for non-anonymous elections.
        /// </summary>
        public long? VoterId { get; set; }
        public DateTime VotedAt { get; set; }
    }
}
