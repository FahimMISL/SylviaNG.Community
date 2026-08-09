namespace SylviaNG.Community.Application.Features.Badges.Models
{
    public class BadgeUpdateRequest
    {
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public bool? IsActive { get; set; }
    }
}
