namespace SylviaNG.Community.Application.Features.Designations.Models
{
    public class DesignationUpdateRequest
    {
        public string? Name { get; set; }
        public string? Grade { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
