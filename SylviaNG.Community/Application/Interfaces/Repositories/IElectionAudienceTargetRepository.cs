using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IElectionAudienceTargetRepository : IRepository<ElectionAudienceTarget>
    {
        Task<List<ElectionAudienceTarget>> GetByElectionIdAsync(long electionId);
    }
}
