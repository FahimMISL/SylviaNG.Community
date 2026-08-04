using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// Self-service "Edit my profile" request (US-1.5). Deliberately does not include
    /// Phone/Email/Extension values - only their visibility - per the story text
    /// ("set Phone/Email/Extension to Public or Private").
    /// </summary>
    public class EmployeeUpdateProfileRequest
    {
        public string? Bio { get; set; }
        public string? Skills { get; set; }
        public string? Interests { get; set; }
        public string? Achievements { get; set; }
        public string? CommunityContributions { get; set; }
        public ContactVisibilityEnum PhoneVisibility { get; set; }
        public ContactVisibilityEnum EmailVisibility { get; set; }
        public ContactVisibilityEnum ExtensionVisibility { get; set; }
    }
}
