using Microsoft.AspNetCore.Http;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        // For Keycloak-issued tokens missing this claim (realm attribute config is unreliable -
        // see EmployeeIdentityEnrichmentMiddleware), it's added onto the ClaimsPrincipal earlier in
        // the pipeline by that middleware, resolved from EmployeeKeycloakAccount instead.
        private const string EmployeeIdClaimType = "employee_id";
        private const string UsernameClaimType = "username";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long? EmployeeId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirst(EmployeeIdClaimType)?.Value;
                return long.TryParse(value, out var employeeId) ? employeeId : null;
            }
        }

        public bool IsHrOrAdmin =>
            _httpContextAccessor.HttpContext?.User?.IsInRole("HR") == true ||
            _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") == true;

        public string? Username =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(UsernameClaimType)?.Value;
    }
}
