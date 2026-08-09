using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// Self-service "Edit my profile" request (US-1.5). Includes Phone/Email/Extension values
    /// and visibility, plus a bundle-diff list of custom contact links: ContactLinks items with
    /// a null Id are added, non-null Ids matching an existing link are updated in place, and any
    /// existing link whose Id is absent from the list is removed - see
    /// EmployeeService.DiffContactLinksAsync for the diff algorithm.
    /// </summary>
    public class EmployeeUpdateProfileRequest
    {
        public string? Bio { get; set; }
        public string? Skills { get; set; }
        public string? Interests { get; set; }
        public string? Achievements { get; set; }
        public string? CommunityContributions { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Extension { get; set; }
        public ContactVisibilityEnum PhoneVisibility { get; set; }
        public ContactVisibilityEnum EmailVisibility { get; set; }
        public ContactVisibilityEnum ExtensionVisibility { get; set; }

        public List<EmployeeContactLinkItem> ContactLinks { get; set; } = new();
    }
}
