using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A single vote cast by an Employee for an ElectionCandidate. Business rules (Election
/// must be open for voting, VotedAt within StartDate/EndDate, and duplicate-vote rejection
/// when the owning Election.AllowMultipleChoice is false) are enforced in ElectionService,
/// not here - this entity is a plain record of the cast vote.
/// </summary>
public class ElectionVote : Audit
{
    public long ElectionVoteId { get; set; }
    public long ElectionId { get; set; }
    public long CandidateId { get; set; }
    public long VoterId { get; set; }
    public DateTime VotedAt { get; set; }
}
