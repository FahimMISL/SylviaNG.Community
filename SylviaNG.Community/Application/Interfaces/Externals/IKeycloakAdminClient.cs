namespace SylviaNG.Community.Application.Interfaces.Externals
{
    /// <summary>Result of a successful Keycloak Direct Access Grant login - see TryPasswordLoginAsync.</summary>
    public class KeycloakLoginResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public string? DisplayName { get; set; }
        public string? Role { get; set; }
        public long? EmployeeId { get; set; }
    }

    /// <summary>
    /// Talks to Keycloak's Admin REST API to provision real Keycloak user accounts for existing
    /// Employees, and to Keycloak's token endpoint to authenticate them via Direct Access Grant
    /// (see TryPasswordLoginAsync) - lets the admin UI's single existing login form authenticate
    /// real employee accounts with no separate redirect-based UI. Requires the configured Keycloak
    /// service account (Keycloak:AdminClientId/AdminClientSecret) to have "manage-users" (and
    /// realm role-mapping) permissions in the target realm - a realm-side configuration this
    /// service cannot grant or verify itself.
    /// </summary>
    public interface IKeycloakAdminClient
    {
        /// <summary>
        /// Creates a Keycloak user with the given password (set as non-temporary - see the note in
        /// KeycloakAdminClient.CreateUserAsync on why a temporary/forced-change credential cannot
        /// work with this app's Direct Access Grant login) and returns Keycloak's own user ID for
        /// the new account. Sets employeeId as the user's "employee_id" attribute so a Keycloak
        /// realm protocol mapper can put it on issued tokens - required for
        /// CurrentUserService.EmployeeId to resolve a Keycloak login back to this Employee record
        /// (see KEYCLOAK_SETUP.md).
        /// </summary>
        Task<string> CreateUserAsync(string username, string? email, string firstName, string lastName, string temporaryPassword, long employeeId);

        /// <summary>
        /// Assigns a realm role (e.g. "Employee") to an existing Keycloak user. The role must
        /// already exist in the realm.
        /// </summary>
        Task AssignRealmRoleAsync(string keycloakUserId, string roleName);

        /// <summary>
        /// Sets a new password for an existing Keycloak user (non-temporary, same reasoning as
        /// CreateUserAsync's initial password) - used for HR-initiated password resets. Does not
        /// affect username, email, or role assignments.
        /// </summary>
        Task ResetPasswordAsync(string keycloakUserId, string newTemporaryPassword);

        /// <summary>
        /// Attempts a Keycloak Direct Access Grant (OAuth2 Resource Owner Password Credentials)
        /// login against Keycloak:ClientId - safe to do server-side (the client secret never
        /// leaves this backend) in a way it would not be from a browser. Requires "Direct Access
        /// Grants Enabled" on that client (see KEYCLOAK_SETUP.md). Returns null for invalid
        /// credentials or a misconfigured client, rather than throwing - callers must treat that
        /// identically to "wrong password" and not leak which store rejected it.
        /// </summary>
        Task<KeycloakLoginResult?> TryPasswordLoginAsync(string username, string password);
    }
}
