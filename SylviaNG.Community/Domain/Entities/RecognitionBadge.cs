using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Join entity between Recognition and Badge. Unique composite index on (RecognitionId, BadgeId)
/// enforced in RecognitionBadgeConfiguration.
/// </summary>
public class RecognitionBadge : Audit
{
    public long RecognitionBadgeId { get; set; }
    public long RecognitionId { get; set; }
    public long BadgeId { get; set; }
}
