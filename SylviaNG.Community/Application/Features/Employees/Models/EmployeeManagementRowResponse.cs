namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// Row DTO for the HR/Admin User Management list (US-1.7), which - unlike the
    /// Directory - includes inactive employees and surfaces IsActive for the Deactivate action.
    /// </summary>
    public class EmployeeManagementRowResponse
    {
        public long EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public long? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? SiteId { get; set; }
        public string? SiteName { get; set; }
        public bool IsActive { get; set; }

        /// <summary>True when this employee already has a Keycloak login account - determines
        /// whether the UI offers "Grant Access" or "Reset Password".</summary>
        public bool HasCredential { get; set; }
    }
}
