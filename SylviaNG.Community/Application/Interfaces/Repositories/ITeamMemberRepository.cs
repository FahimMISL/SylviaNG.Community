using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ITeamMemberRepository : IRepository<TeamMember>
    {
        Task<bool> ExistsAsync(long teamId, long employeeId);
        Task<TeamMember?> GetAsync(long teamId, long employeeId);
        Task<List<TeamMember>> GetByTeamIdAsync(long teamId);
    }
}
