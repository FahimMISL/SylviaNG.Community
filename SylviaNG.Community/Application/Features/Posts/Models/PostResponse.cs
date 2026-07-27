namespace SylviaNG.Community.Application.Features.Posts.Models
{
    public class PostResponse
    {
        public long PostId { get; set; }
        public long EmployeeId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public string? Content { get; set; }
        public bool IsAnnouncement { get; set; }
        public bool IsPoll { get; set; }
        public bool IsLocked { get; set; }
        public bool IsHidden { get; set; }
        public DateTime? CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
    }
}
