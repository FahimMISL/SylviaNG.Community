namespace SylviaNG.Community.Application.Features.Announcements.Models
{
    public class AnnouncementLookupResponse
    {
        public long AnnouncementId { get; set; }
        public string? Title { get; set; }

        public string CodeName =>
            (!string.IsNullOrWhiteSpace(Title))
                ? $"{AnnouncementId} - {Title}"
                : Title ?? string.Empty;
    }
}