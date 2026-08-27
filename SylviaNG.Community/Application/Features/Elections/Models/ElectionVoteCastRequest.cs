namespace SylviaNG.Community.Application.Features.Elections.Models
{
    /// <summary>
    /// One atomic ballot submission - CandidateIds holds every candidate the voter selected
    /// in a single call (exactly 1 for a single-choice election; between the election's
    /// MinSelection and MaxSelection for a multiple-choice election). See
    /// ElectionService.CastVoteAsync for how this is turned into one ElectionVote row per
    /// selected candidate.
    /// </summary>
    public class ElectionVoteCastRequest
    {
        public List<long> CandidateIds { get; set; } = new();
    }
}
