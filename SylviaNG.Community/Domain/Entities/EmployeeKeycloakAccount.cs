using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Records that an Employee has a real Keycloak login account, provisioned via Keycloak's Admin
/// API by HR/Admin (see EmployeeCredentialService). Deliberately holds no password/hash - Keycloak
/// owns the credential material entirely; this is a local audit/link record only. Unrelated to
/// Credential/InMemoryCredentialRepository, which back a separate, local-only login system.
/// </summary>
public class EmployeeKeycloakAccount : Audit
{
    public long EmployeeKeycloakAccountId { get; set; }
    public long EmployeeId { get; set; }
    public string KeycloakUserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string AssignedRole { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
