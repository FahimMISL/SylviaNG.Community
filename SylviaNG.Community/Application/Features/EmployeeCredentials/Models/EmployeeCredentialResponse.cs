namespace SylviaNG.Community.Application.Features.EmployeeCredentials.Models
{
    /// <summary>Deliberately never echoes the temporary password back.</summary>
    public class EmployeeCredentialResponse
    {
        public long EmployeeId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string KeycloakUserId { get; set; } = string.Empty;
        public string AssignedRole { get; set; } = string.Empty;
    }
}
