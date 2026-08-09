namespace SylviaNG.Community.Application.Features.Departments.Models
{
    public class DepartmentCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Description { get; set; }
    }
}
