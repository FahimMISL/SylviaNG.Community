namespace SylviaNG.Community.Application.Features.Posts.Models
{
    public class PostUpdateRequest
    {
        public string? Type { get; set; }
        public string? Visibility { get; set; }
        public string? Content { get; set; }
        public bool? IsAnnouncement { get; set; }
    }
}
