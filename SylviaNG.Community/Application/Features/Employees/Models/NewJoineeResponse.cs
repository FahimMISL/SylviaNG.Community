namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// Feed sidebar "New Joinees" card row. Live-filtered on every request to employees who
    /// joined within the last 2 days - see EmployeeService.GetNewJoineesAsync.
    /// </summary>
    public class NewJoineeResponse
    {
        public long EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? PhotoUrl { get; set; }
        public DateOnly DateOfJoining { get; set; }
    }
}
