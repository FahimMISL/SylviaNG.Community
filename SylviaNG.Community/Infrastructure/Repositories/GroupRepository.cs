using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Group>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SearchProperties ??= new[] { nameof(Group.Name), nameof(Group.Description) };
            request.SortBy ??= nameof(Group.Name);

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
