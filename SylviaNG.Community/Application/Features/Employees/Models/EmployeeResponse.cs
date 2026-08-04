using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// Full profile (US-1.3 colleague view / US-1.4 my-profile view). Phone/Email/Extension
    /// are null unless the viewer is HR/Admin or the profile owner - see EmployeeMapper.ToResponse.
    /// Badges and RecentRecognitions have no backing feature yet (Recognition/Gamification
    /// aren't built) and are always empty - the frontend renders their empty-state for them.
    /// </summary>
    public class EmployeeResponse
    {
        public long EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }

        public long? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? SiteId { get; set; }
        public string? SiteName { get; set; }
        public long? GradeId { get; set; }
        public string? GradeName { get; set; }
        public string? Division { get; set; }

        public string? Bio { get; set; }
        public List<string> Skills { get; set; } = new();
        public List<string> Interests { get; set; } = new();
        public List<string> Achievements { get; set; } = new();
        public List<string> CommunityContributions { get; set; } = new();
        public List<string> Badges { get; set; } = new();
        public List<string> RecentRecognitions { get; set; } = new();
        public string? PhotoUrl { get; set; }
        public string? CoverPhotoUrl { get; set; }
        public bool IsActive { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Extension { get; set; }
        public ContactVisibilityEnum PhoneVisibility { get; set; }
        public ContactVisibilityEnum EmailVisibility { get; set; }
        public ContactVisibilityEnum ExtensionVisibility { get; set; }

        public bool IsOwnProfile { get; set; }
    }
}
