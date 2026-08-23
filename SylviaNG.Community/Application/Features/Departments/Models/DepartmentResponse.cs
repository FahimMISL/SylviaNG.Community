namespace SylviaNG.Community.Application.Features.Departments.Models
{
    public class DepartmentResponse
    {
        public long DepartmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Description { get; set; }
        public long? CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
