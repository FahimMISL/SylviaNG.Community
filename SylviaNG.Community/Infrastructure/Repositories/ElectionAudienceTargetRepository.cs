using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ElectionAudienceTargetRepository : Repository<ElectionAudienceTarget>, IElectionAudienceTargetRepository
    {
        public ElectionAudienceTargetRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<ElectionAudienceTarget>> GetByElectionIdAsync(long electionId)
        {
            return await _dbSet.Where(t => t.ElectionId == electionId).ToListAsync();
        }
    }
}
