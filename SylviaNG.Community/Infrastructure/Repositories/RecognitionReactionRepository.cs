using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class RecognitionReactionRepository : Repository<RecognitionReaction>, IRecognitionReactionRepository
    {
        public RecognitionReactionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(long recognitionId, long employeeId)
        {
            return await _dbSet.AnyAsync(rr => rr.RecognitionId == recognitionId && rr.EmployeeId == employeeId);
        }

        public async Task<RecognitionReaction?> GetAsync(long recognitionId, long employeeId)
        {
            return await _dbSet.FirstOrDefaultAsync(rr => rr.RecognitionId == recognitionId && rr.EmployeeId == employeeId);
        }

        public async Task<List<RecognitionReaction>> GetByRecognitionIdAsync(long recognitionId)
        {
            return await _dbSet.Where(rr => rr.RecognitionId == recognitionId).ToListAsync();
        }
    }
}
