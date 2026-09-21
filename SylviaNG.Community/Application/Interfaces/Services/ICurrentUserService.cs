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

        /// <summary>
        /// Same as <see cref="EmployeeId"/> but throws <see cref="Application.Common.Exceptions.ForbiddenException"/>
        /// (mapped to HTTP 403) when the caller has no employee identity - e.g. an Admin system account.
        /// Use this at any call site that would otherwise need to fall back to a fake id like 0.
        /// </summary>
        long RequireEmployeeId();

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
