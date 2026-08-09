using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class SurveyOptionRepository : Repository<SurveyOption>, ISurveyOptionRepository
    {
        public SurveyOptionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<SurveyOption>> GetByQuestionIdAsync(long questionId)
        {
            return await _dbSet
                .Where(o => o.QuestionId == questionId)
                .OrderBy(o => o.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<SurveyOption>> GetByQuestionIdsAsync(IEnumerable<long> questionIds)
        {
            return await _dbSet
                .Where(o => questionIds.Contains(o.QuestionId))
                .OrderBy(o => o.DisplayOrder)
                .ToListAsync();
        }
    }
}
