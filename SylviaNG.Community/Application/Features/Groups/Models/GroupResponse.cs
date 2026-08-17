using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Groups.Models
{
    public class GroupResponse
    {
        public long GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public GroupVisibilityEnum Visibility { get; set; }
        public int MemberCount { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
