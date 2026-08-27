using System.Security.Claims;
using SylviaNG.Community.Application.Interfaces.Repositories;

namespace SylviaNG.Community.Middlewares
{
    /// <summary>
    /// Resolves "employee_id" for authenticated requests whose JWT is missing that claim, by
    /// looking up the token's Keycloak subject ("sub", mapped to ClaimTypes.NameIdentifier under
    /// ASP.NET Core's default inbound claim mapping) against EmployeeKeycloakAccount.KeycloakUserId.
    ///
    /// This exists because Keycloak-issued tokens can't be relied on to carry a custom "employee_id"
    /// attribute claim - that depends on realm-side User Profile/protocol-mapper configuration that
    /// has already been found to silently drop the attribute. The Keycloak subject, by contrast, is
    /// always present and stable on any valid token, and this app already stores the EmployeeId<->
    /// KeycloakUserId link locally (set at Grant Access time - see EmployeeCredentialService), so
    /// resolving identity through it here removes the dependency on that fragile realm config for
    /// something as core as "which employee is this".
    ///
    /// A no-op for local-JWT users (their NameIdentifier is a username, which won't match any
    /// KeycloakUserId) and for any request that already carries "employee_id" or isn't authenticated.
    /// </summary>
    public class EmployeeIdentityEnrichmentMiddleware
    {
        private const string EmployeeIdClaimType = "employee_id";

        private readonly RequestDelegate _next;

        public EmployeeIdentityEnrichmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IEmployeeKeycloakAccountRepository employeeKeycloakAccountRepository)
        {
            if (context.User.Identity?.IsAuthenticated == true
                && context.User.Identity is ClaimsIdentity identity
                && identity.FindFirst(EmployeeIdClaimType) == null)
            {
                var keycloakUserId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(keycloakUserId))
                {
                    var account = await employeeKeycloakAccountRepository.GetByKeycloakUserIdAsync(keycloakUserId);
                    if (account != null && account.IsActive)
                    {
                        identity.AddClaim(new Claim(EmployeeIdClaimType, account.EmployeeId.ToString()));
                    }
                }
            }

            await _next(context);
        }
    }
}
