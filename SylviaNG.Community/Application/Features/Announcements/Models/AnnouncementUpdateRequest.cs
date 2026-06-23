using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Announcements.Models
{
    public class AnnouncementUpdateRequest
    {
        public long? DepartmentId { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public AnnouncementTypeEnum? AnnouncementType { get; set; }
        public AnnouncementStatusEnum? Status { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsActive { get; set; }
    }
}