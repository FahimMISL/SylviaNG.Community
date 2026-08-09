using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class RecognitionBadgeRepository : Repository<RecognitionBadge>, IRecognitionBadgeRepository
    {
        public RecognitionBadgeRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<RecognitionBadge>> GetByRecognitionIdAsync(long recognitionId)
        {
            return await _dbSet.Where(x => x.RecognitionId == recognitionId).ToListAsync();
        }

        public async Task<List<RecognitionBadge>> GetByRecognitionIdsAsync(IEnumerable<long> recognitionIds)
        {
            return await _dbSet.Where(x => recognitionIds.Contains(x.RecognitionId)).ToListAsync();
        }
    }
}
