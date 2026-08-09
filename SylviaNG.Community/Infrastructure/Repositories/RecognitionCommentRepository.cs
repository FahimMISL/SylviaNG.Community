using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class RecognitionCommentRepository : Repository<RecognitionComment>, IRecognitionCommentRepository
    {
        public RecognitionCommentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<RecognitionComment>> GetByRecognitionIdAsync(long recognitionId)
        {
            return await _dbSet.Where(rc => rc.RecognitionId == recognitionId).ToListAsync();
        }
    }
}
