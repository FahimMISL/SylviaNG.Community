namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// Self-service "set my profile photo" request. Carries the StoragePath already returned
    /// by POST community/file-upload (module "employee-photo") - this endpoint only persists
    /// that path onto Employee.PhotoUrl, it does not accept file bytes itself.
    /// </summary>
    public class EmployeeUpdatePhotoRequest
    {
        public string StoragePath { get; set; } = string.Empty;
    }
}
