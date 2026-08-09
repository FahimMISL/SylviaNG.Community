using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Groups.Models
{
    public class GroupUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public GroupVisibilityEnum? Visibility { get; set; }
    }
}
