using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Groups.Models
{
    public class GroupCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public GroupVisibilityEnum Visibility { get; set; } = GroupVisibilityEnum.Public;
    }
}
