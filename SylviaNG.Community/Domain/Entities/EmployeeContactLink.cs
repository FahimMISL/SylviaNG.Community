using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A custom contact link (LinkedIn, Facebook, personal site, etc.) owned by an Employee's
/// profile. Platform is free text - the predefined dropdown with an "Other" option is a
/// frontend-only concern.
/// </summary>
public class EmployeeContactLink : Audit
{
    public long EmployeeContactLinkId { get; set; }
    public long EmployeeId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public ContactVisibilityEnum Visibility { get; set; } = ContactVisibilityEnum.Private;
}
