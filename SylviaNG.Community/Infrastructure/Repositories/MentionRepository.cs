using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class MentionRepository : Repository<Mention>, IMentionRepository
    {
        public MentionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Mention>> GetPaginatedForEmployeeAsync(long mentionedEmployeeId, PagedRequest request)
        {
            var query = _dbSet.Where(m => m.MentionedEmployeeId == mentionedEmployeeId).AsQueryable();

            request.SortBy ??= nameof(Mention.CreatedAt);
            request.SortDirection ??= "desc";

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
