namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// Self-service "set my cover photo" request. Carries the StoragePath already returned
    /// by POST community/file-upload (module "employee-cover") - this endpoint only persists
    /// that path onto Employee.CoverPhotoUrl, it does not accept file bytes itself.
    /// </summary>
    public class EmployeeUpdateCoverPhotoRequest
    {
        public string StoragePath { get; set; } = string.Empty;
    }
}
