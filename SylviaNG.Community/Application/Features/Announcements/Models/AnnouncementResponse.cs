using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Announcements.Models
{
    public class AnnouncementResponse
    {
        public long AnnouncementId { get; set; }
        public long SiteId { get; set; }
        public string? SiteName { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Body { get; set; }
        public AnnouncementTypeEnum AnnouncementType { get; set; }
        public AnnouncementStatusEnum Status { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}