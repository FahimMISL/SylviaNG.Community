using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {
        (string AccessToken, DateTime ExpiresAtUtc) GenerateToken(Credential credential);
    }
}
