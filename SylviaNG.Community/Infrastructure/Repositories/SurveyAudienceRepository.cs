using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class SurveyAudienceRepository : Repository<SurveyAudience>, ISurveyAudienceRepository
    {
        public SurveyAudienceRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<SurveyAudience>> GetBySurveyIdAsync(long surveyId)
        {
            return await _dbSet.Where(a => a.SurveyId == surveyId).ToListAsync();
        }
    }
}
