using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ChatReportRepository : Repository<ChatReport>, IChatReportRepository
    {
        public ChatReportRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<ChatReport>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SortBy ??= nameof(ChatReport.CreatedAt);
            request.SortDirection ??= "desc";

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
