using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Announcements.Models
{
    public class AnnouncementCreateRequest
    {
        public long SiteId { get; set; }
        public long? DepartmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Body { get; set; }
        public AnnouncementTypeEnum AnnouncementType { get; set; } = AnnouncementTypeEnum.General;
        public DateTime? PublishDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}