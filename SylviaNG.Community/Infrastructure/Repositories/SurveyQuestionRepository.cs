using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class SurveyQuestionRepository : Repository<SurveyQuestion>, ISurveyQuestionRepository
    {
        public SurveyQuestionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<SurveyQuestion>> GetBySurveyIdAsync(long surveyId)
        {
            return await _dbSet
                .Where(q => q.SurveyId == surveyId)
                .OrderBy(q => q.DisplayOrder)
                .ToListAsync();
        }
    }
}
