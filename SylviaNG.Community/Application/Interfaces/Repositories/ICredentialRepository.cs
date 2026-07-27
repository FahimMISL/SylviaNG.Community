using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ICredentialRepository
    {
        Task<Credential?> GetByUsernameAsync(string username);
    }
}
