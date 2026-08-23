using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class SurveyResponseRepository : Repository<SurveyResponse>, ISurveyResponseRepository
    {
        public SurveyResponseRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(long surveyId, long employeeId)
        {
            return await _dbSet.AnyAsync(r => r.SurveyId == surveyId && r.EmployeeId == employeeId);
        }

        public async Task<PagedResult<SurveyResponse>> GetPaginatedBySurveyIdAsync(long surveyId, PagedRequest request)
        {
            var query = _dbSet.Where(r => r.SurveyId == surveyId);

            request.SearchProperties ??= new[] { nameof(SurveyResponse.CompletionStatus) };

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<List<SurveyResponse>> GetAllBySurveyIdAsync(long surveyId)
        {
            return await _dbSet.Where(r => r.SurveyId == surveyId).ToListAsync();
        }
    }
}
