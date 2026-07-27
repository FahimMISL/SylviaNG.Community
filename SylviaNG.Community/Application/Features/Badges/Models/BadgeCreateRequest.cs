namespace SylviaNG.Community.Application.Features.Badges.Models
{
    public class BadgeCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Description { get; set; }
    }
}
