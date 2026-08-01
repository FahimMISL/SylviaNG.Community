using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Auth.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Services
{
    public class AuthService : IAuthService
    {
        private const string InvalidCredentialsMessage = "Invalid username or password.";

        private readonly ICredentialRepository _credentialRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(ICredentialRepository credentialRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _credentialRepository = credentialRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var credential = await _credentialRepository.GetByUsernameAsync(request.Username);

            // Same message whether the username doesn't exist or the password is wrong,
            // to avoid leaking which usernames are valid.
            if (credential == null || !BCrypt.Net.BCrypt.Verify(request.Password, credential.PasswordHash))
                throw new UnauthorizedException(InvalidCredentialsMessage);

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
