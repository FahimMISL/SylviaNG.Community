using SylviaNG.Community.Application.Features.Auth.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
