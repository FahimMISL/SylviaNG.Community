using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SylviaNG.Community.Infrastructure.Authentication
{
    /// <summary>
    /// Issues locally-signed JWTs for the admin UI's login page (see AuthController). Produces
    /// the same claim shape as DevHeaderAuthenticationHandler (employee_id, role, tenant_id) so
    /// the rest of the app - CurrentUserService, HRAdminOnly policy, multi-tenancy - keeps
    /// working unchanged regardless of whether the caller authenticated via Keycloak or here.
    /// Validated by the "Local" JwtBearer scheme registered in AuthenticationExtensions.
    /// </summary>
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string AccessToken, DateTime ExpiresAtUtc) GenerateToken(Credential credential)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var issuer = jwtSection.GetValue<string>("Issuer") ?? throw new ArgumentNullException("Jwt:Issuer");
            var audience = jwtSection.GetValue<string>("Audience") ?? throw new ArgumentNullException("Jwt:Audience");
            var signingKey = jwtSection.GetValue<string>("SigningKey") ?? throw new ArgumentNullException("Jwt:SigningKey");
            var expiryMinutes = jwtSection.GetValue<int>("AccessTokenExpiryMinutes", 60);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, credential.EmployeeId?.ToString() ?? credential.Username),
                new(ClaimTypes.Name, credential.DisplayName),
                new(ClaimTypes.Role, credential.Role),
                new("tenant_id", "default_tenant")
            };

            if (credential.EmployeeId.HasValue)
                claims.Add(new Claim("employee_id", credential.EmployeeId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: signingCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return (accessToken, expiresAtUtc);
        }
    }
}
