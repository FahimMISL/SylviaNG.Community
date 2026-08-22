namespace SylviaNG.Community.Application.Interfaces.Externals
{
    /// <summary>
    /// Talks to Keycloak's Admin REST API (not the OIDC login flow) to provision real Keycloak
    /// user accounts for existing Employees. Requires the configured Keycloak service account
    /// (Keycloak:AdminClientId/AdminClientSecret) to have "manage-users" (and realm role-mapping)
    /// permissions in the target realm - a realm-side configuration this service cannot grant or
    /// verify itself.
    /// </summary>
    public interface IKeycloakAdminClient
    {
        /// <summary>
        /// Creates a Keycloak user with a temporary password (forces a change at first login) and
        /// returns Keycloak's own user ID for the new account.
        /// </summary>
        Task<string> CreateUserAsync(string username, string? email, string firstName, string lastName, string temporaryPassword);

        /// <summary>
        /// Assigns a realm role (e.g. "Employee") to an existing Keycloak user. The role must
        /// already exist in the realm.
        /// </summary>
        Task AssignRealmRoleAsync(string keycloakUserId, string roleName);

        /// <summary>
        /// Sets a new temporary password for an existing Keycloak user (forces a change at next
        /// login, same as CreateUserAsync's initial password) - used for HR-initiated password
        /// resets. Does not affect username, email, or role assignments.
        /// </summary>
        Task ResetPasswordAsync(string keycloakUserId, string newTemporaryPassword);
    }
}
