using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// One row of the contact-links bundle submitted with EmployeeUpdateProfileRequest.
    /// Id is null for a new link to add; non-null for an existing link to update. Any
    /// existing DB row not represented in the submitted list is removed - see
    /// EmployeeService.DiffContactLinksAsync.
    /// </summary>
    public class EmployeeContactLinkItem
    {
        public long? Id { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public ContactVisibilityEnum Visibility { get; set; }
    }
}
