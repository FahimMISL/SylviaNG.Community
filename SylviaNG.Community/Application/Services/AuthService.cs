using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Auth.Models;
using SylviaNG.Community.Application.Interfaces.Externals;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Services
{
    public class AuthService : IAuthService
    {
        private const string InvalidCredentialsMessage = "Invalid username or password.";

        private readonly ICredentialRepository _credentialRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IKeycloakAdminClient _keycloakAdminClient;

        public AuthService(ICredentialRepository credentialRepository, IJwtTokenGenerator jwtTokenGenerator, IKeycloakAdminClient keycloakAdminClient)
        {
            _credentialRepository = credentialRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _keycloakAdminClient = keycloakAdminClient;
        }

        /// <summary>
        /// Checks the local Credential store first (unchanged behavior for the demo accounts);
        /// if that fails (unknown username or wrong local password), falls through to a Keycloak
        /// Direct Access Grant using the same submitted username/password - lets employees granted
        /// access via EmployeeCredentialController's Grant Access/Reset Password log in through
        /// this exact same form, no separate Keycloak UI. Both failure paths throw the identical
        /// UnauthorizedException below, so neither leaks which store (or whether the username
        /// exists in either) rejected the attempt.
        /// </summary>
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var credential = await _credentialRepository.GetByUsernameAsync(request.Username);
            if (credential != null && BCrypt.Net.BCrypt.Verify(request.Password, credential.PasswordHash))
            {
                var (accessToken, expiresAtUtc) = _jwtTokenGenerator.GenerateToken(credential);
                return new LoginResponseDto
                {
                    AccessToken = accessToken,
                    ExpiresAtUtc = expiresAtUtc,
                    Username = credential.Username,
                    DisplayName = credential.DisplayName,
                    Role = credential.Role,
                    EmployeeId = credential.EmployeeId
                };
            }

            var keycloakResult = await _keycloakAdminClient.TryPasswordLoginAsync(request.Username, request.Password);
            if (keycloakResult != null)
            {
                return new LoginResponseDto
                {
                    AccessToken = keycloakResult.AccessToken,
                    ExpiresAtUtc = keycloakResult.ExpiresAtUtc,
                    Username = request.Username,
                    DisplayName = keycloakResult.DisplayName ?? request.Username,
                    Role = keycloakResult.Role ?? "Employee",
                    EmployeeId = keycloakResult.EmployeeId
                };
            }

            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        public async Task ChangePasswordAsync(string username, ChangePasswordRequestDto request)
        {
            var credential = await _credentialRepository.GetByUsernameAsync(username);

            if (credential == null || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, credential.PasswordHash))
                throw new UnauthorizedException("Current password is incorrect.");

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 11);
            await _credentialRepository.UpdatePasswordHashAsync(username, newPasswordHash);
        }
    }
}
