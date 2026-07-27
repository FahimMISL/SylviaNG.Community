namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// Lightweight card DTO for the Directory grid (US-1.1/1.2). Deliberately excludes bio/
    /// contact/achievement fields - a browse grid has no business receiving private data.
    /// </summary>
    public class EmployeeDirectoryCardResponse
    {
        public long EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? PhotoUrl { get; set; }
        public long? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? SiteId { get; set; }
        public string? SiteName { get; set; }
        public List<string> Skills { get; set; } = new();
    }
}
