namespace SylviaNG.Community.Application.Features.Departments.Models
{
    public class DepartmentUpdateRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
