using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Posts.Models
{
    public class PostCreateRequest
    {
        public long EmployeeId { get; set; }

        /// <summary>Set to post inside a Group instead of the main feed - see PostService.CreateAsync for the membership check this triggers.</summary>
        public long? GroupId { get; set; }

        public string Type { get; set; } = string.Empty;
        public VisibilityEnum Visibility { get; set; }
        public string? Content { get; set; }
        public bool IsAnnouncement { get; set; }
        public bool IsPoll { get; set; }
        public List<long>? MentionedEmployeeIds { get; set; }
    }
}
