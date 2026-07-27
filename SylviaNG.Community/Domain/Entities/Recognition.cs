using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A peer-to-peer or HR-issued recognition sent from one employee to another.
/// </summary>
public class Recognition : Audit
{
    public long RecognitionId { get; set; }
    public long SenderId { get; set; }
    public long RecipientId { get; set; }
    public string RecognitionType { get; set; } = string.Empty;
    public string? CoreValue { get; set; }
    public string? AwardTitle { get; set; }
    public string? Message { get; set; }
    public bool IsPublic { get; set; } = true;
}
