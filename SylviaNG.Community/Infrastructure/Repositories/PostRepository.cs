using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Post>> GetFeedPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.Where(p => !p.IsHidden).AsQueryable();

            request.SearchProperties ??= new[] { nameof(Post.Content) };
            request.SortBy ??= nameof(Post.CreatedAt);
            request.SortDirection ??= "desc";

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
