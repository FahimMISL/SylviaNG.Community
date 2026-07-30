namespace SylviaNG.Community.Application.Interfaces.Services
{
    /// <summary>
    /// Resolves the calling user's identity/role from the authenticated request.
    /// Backed by JWT claims in production; see DevHeaderAuthenticationHandler for the
    /// Development-only fallback that lets the frontend's mock persona switcher exercise
    /// this without a real Keycloak login flow.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Employee id of the caller, if the caller is an Employee-type account (Employee/Supervisor/HR).
        /// Null for Admin accounts, which are not Employee records, or when unauthenticated.
        /// </summary>
        long? EmployeeId { get; }

        bool IsHrOrAdmin { get; }

        /// <summary>
        /// Username of the caller, if authenticated via the locally-issued JWT ("Local" scheme -
        /// see JwtTokenGenerator). Null for Keycloak-authenticated or Development DevHeader
        /// sessions, which carry no local credential record - only locally-authenticated
        /// accounts can use self-service actions like change-password.
        /// </summary>
        string? Username { get; }
    }
}
