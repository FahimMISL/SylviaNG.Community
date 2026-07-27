namespace SylviaNG.Community.Application.Features.Badges.Models
{
    public class BadgeResponse
    {
        public long BadgeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Description { get; set; }
    }
}
