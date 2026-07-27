namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Local login credential for the admin UI's real (JWT-based) login flow - kept as a static
/// in-memory list (see InMemoryCredentialRepository) rather than a database table, since no
/// database is provisioned yet. Separate from Employee, which is synced externally from the
/// Core microservice via Kafka and has no password field. Admin is a system account
/// (EmployeeId null), matching the role glossary.
/// </summary>
public class Credential
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public long? EmployeeId { get; set; }
    public bool IsActive { get; set; } = true;
}
