namespace SylviaNG.Community.Application.Features.EmployeeCredentials.Models
{
    public class EmployeeCredentialCreateRequest
    {
        public long EmployeeId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string TemporaryPassword { get; set; } = string.Empty;

        /// <summary>Keycloak realm role to assign. Defaults to "Employee" when not supplied.</summary>
        public string? Role { get; set; }
    }
}
