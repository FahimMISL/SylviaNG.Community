using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

public class PollVote : Audit
{
    public long VoteId { get; set; }
    public long PollOptionId { get; set; }
    public long EmployeeId { get; set; }
    public DateTime VotedAt { get; set; }
}
