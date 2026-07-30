using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ICredentialRepository
    {
        Task<Credential?> GetByUsernameAsync(string username);

        System.Threading.Tasks.Task UpdatePasswordHashAsync(string username, string newPasswordHash);
    }
}
