namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// HR/Admin "Edit Employee" request (User Management). Scoped to locally-owned fields
    /// only - Email, DateOfBirth, DateOfJoining. EmployeeName/EmployeeCode/DepartmentId/
    /// DesignatioId/SiteId are deliberately excluded: they're owned by the Core/Employee
    /// microservice and a local edit here would be overwritten by the next sync.
    /// </summary>
    public class EmployeeUpdateRequest
    {
        public string Email { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public DateOnly DateOfJoining { get; set; }
    }
}
