using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class SurveyRepository : Repository<Survey>, ISurveyRepository
    {
        public SurveyRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByTitleAsync(string title, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(s => s.Title == title && (!excludeId.HasValue || s.SurveyId != excludeId.Value));
        }

        public async Task<PagedResult<Survey>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SearchProperties ??= new[] { nameof(Survey.Title), nameof(Survey.Description) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
