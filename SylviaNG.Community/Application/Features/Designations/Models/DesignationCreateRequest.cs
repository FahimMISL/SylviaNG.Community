namespace SylviaNG.Community.Application.Features.Designations.Models
{
    public class DesignationCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Grade { get; set; }
        public string? Description { get; set; }
    }
}
