namespace SylviaNG.Community.Application.Features.Designations.Models
{
    public class DesignationResponse
    {
        public long DesignationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Grade { get; set; }
        public string? Description { get; set; }
        public long? CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
