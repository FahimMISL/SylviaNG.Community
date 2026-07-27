namespace SylviaNG.Community.Application.Features.Posts.Models
{
    public class PostCreateRequest
    {
        public long EmployeeId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public string? Content { get; set; }
        public bool IsAnnouncement { get; set; }
        public bool IsPoll { get; set; }
    }
}
