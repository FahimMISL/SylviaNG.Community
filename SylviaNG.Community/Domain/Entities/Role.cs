using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Standalone reference data only - not wired to Credential.Role, JWT role claims,
/// or the HRAdminOnly policy, which remain string-based and untouched.
/// </summary>
public class Role : Audit
{
    public long RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
