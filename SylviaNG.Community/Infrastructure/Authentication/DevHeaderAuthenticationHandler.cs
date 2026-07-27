using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SylviaNG.Community.Infrastructure.Authentication
{
    /// <summary>
    /// Development-only authentication scheme that synthesizes a ClaimsPrincipal from
    /// X-Dev-Employee-Id / X-Dev-Role request headers, so the frontend's mock "acting as"
    /// persona switcher can exercise real backend authorization end to end without a
    /// Keycloak login flow (which doesn't exist in the admin UI yet). Only ever registered
    /// when the host environment is Development (see
    /// AuthenticationExtensions.AddKeycloakJwtAuthentication) - never active outside it,
    /// where only the Keycloak JWT bearer scheme is registered.
    /// </summary>
    public class DevHeaderAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "DevHeader";

        private const string EmployeeIdHeader = "X-Dev-Employee-Id";
        private const string RoleHeader = "X-Dev-Role";

        public DevHeaderAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var employeeIdHeader = Request.Headers[EmployeeIdHeader].FirstOrDefault();
            var roleHeader = Request.Headers[RoleHeader].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(employeeIdHeader) && string.IsNullOrWhiteSpace(roleHeader))
                return Task.FromResult(AuthenticateResult.NoResult());

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, employeeIdHeader ?? roleHeader ?? "dev-user"),
                new("tenant_id", "default_tenant")
            };

            if (!string.IsNullOrWhiteSpace(employeeIdHeader))
                claims.Add(new Claim("employee_id", employeeIdHeader));

            if (!string.IsNullOrWhiteSpace(roleHeader))
                claims.Add(new Claim(ClaimTypes.Role, roleHeader));

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
