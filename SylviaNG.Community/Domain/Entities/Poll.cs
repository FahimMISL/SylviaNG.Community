using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A poll attached 1:1 to a Post that has IsPoll = true.
/// </summary>
public class Poll : Audit
{
    public long PollId { get; set; }
    public long PostId { get; set; }
    public bool AllowVoteChange { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
