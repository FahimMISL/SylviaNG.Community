using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Represents an HR/organization announcement published to employees in the community.
/// </summary>
public class Announcement : Audit
{
    public long AnnouncementId { get; set; }
    public long SiteId { get; set; }
    public long? DepartmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public AnnouncementTypeEnum AnnouncementType { get; set; } = AnnouncementTypeEnum.General;
    public new AnnouncementStatusEnum Status { get; set; } = AnnouncementStatusEnum.Draft;
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
}