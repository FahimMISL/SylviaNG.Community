using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class SurveyAnswerRepository : Repository<SurveyAnswer>, ISurveyAnswerRepository
    {
        public SurveyAnswerRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<SurveyAnswer>> GetByResponseIdAsync(long responseId)
        {
            return await _dbSet.Where(a => a.ResponseId == responseId).ToListAsync();
        }

        public async Task<List<SurveyAnswer>> GetByResponseIdsAsync(IEnumerable<long> responseIds)
        {
            return await _dbSet.Where(a => responseIds.Contains(a.ResponseId)).ToListAsync();
        }
    }
}
