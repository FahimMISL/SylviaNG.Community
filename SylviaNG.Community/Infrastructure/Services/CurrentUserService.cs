using Microsoft.AspNetCore.Http;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        // ASSUMPTION: the real Keycloak realm maps an "employee_id" claim onto the JWT.
        // Confirm against the actual Keycloak client/claim-mapper configuration once
        // available and adjust this single constant if the real claim name differs.
        private const string EmployeeIdClaimType = "employee_id";

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
    }
}
