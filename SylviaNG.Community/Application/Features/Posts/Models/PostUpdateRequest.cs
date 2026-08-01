using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Posts.Models
{
    public class PostUpdateRequest
    {
        public string? Type { get; set; }
        public VisibilityEnum? Visibility { get; set; }
        public string? Content { get; set; }
        public bool? IsAnnouncement { get; set; }
        public List<long>? MentionedEmployeeIds { get; set; }
    }
}
