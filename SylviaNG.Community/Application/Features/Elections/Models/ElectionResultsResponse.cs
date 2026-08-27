namespace SylviaNG.Community.Application.Features.Elections.Models
{
    public class ElectionCandidateTally
    {
        public long ElectionCandidateId { get; set; }
        public long? EmployeeId { get; set; }
        public long? TeamId { get; set; }
        public int VoteCount { get; set; }
    }

    /// <summary>Only populated for identified (non-anonymous) elections - see ElectionResultsResponse.VoterDetails.</summary>
    public class ElectionVoterDetail
    {
        public long VoterId { get; set; }
        public List<long> CandidateIds { get; set; } = new();
        public DateTime VotedAt { get; set; }
    }

    /// <summary>
    /// US-9.12: aggregated per-candidate totals always shown; VoterDetails is left null (never
    /// computed) for anonymous elections so there is nothing to leak, and populated only when
    /// !IsAnonymous.
    /// </summary>
    public class ElectionResultsResponse
    {
        public long ElectionId { get; set; }
        public bool IsAnonymous { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public List<ElectionCandidateTally> CandidateTallies { get; set; } = new();
        public List<ElectionVoterDetail>? VoterDetails { get; set; }
    }
}
