using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Employee directory/profile record (Feature 1). Core identity fields
/// (EmployeeName, EmployeeCode, DepartmentId, DesignatioId, SiteId) are
/// owned by the Core/Employee microservice and populated here manually / via the
/// Core gRPC client (ICoreGrpcClient) rather than an event stream; the profile
/// fields below are owned locally.
/// </summary>
public class Employee : Audit
{
    public long EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeCode { get; set; }
    public long? DepartmentId { get; set; }
    public long? DesignatioId { get; set; }
    public long? SiteId { get; set; }

    // Feature 1: Employee Profiles & Directory
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Extension { get; set; }
    public ContactVisibilityEnum PhoneVisibility { get; set; } = ContactVisibilityEnum.Private;
    public ContactVisibilityEnum EmailVisibility { get; set; } = ContactVisibilityEnum.Private;
    public ContactVisibilityEnum ExtensionVisibility { get; set; } = ContactVisibilityEnum.Private;
    public string? Division { get; set; }
    public string? Bio { get; set; }
    public string? Skills { get; set; }
    public string? Interests { get; set; }
    public string? Achievements { get; set; }
    public string? CommunityContributions { get; set; }
    public string? PhotoUrl { get; set; }
    public string? CoverPhotoUrl { get; set; }
    public long? PhotoFileId { get; set; }
    public long? CoverPhotoFileId { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Settable at EmployeeCreate time, editable afterward via the self-service profile
    /// flow or HR/Admin's Edit Employee dialog. Only Month/Day are ever exposed outside the
    /// owner/HR-only branch of EmployeeMapper.ToResponse.</summary>
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>Set at EmployeeCreate time, editable afterward via HR/Admin's Edit Employee
    /// dialog (User Management).</summary>
    public DateOnly? DateOfJoining { get; set; }
}
