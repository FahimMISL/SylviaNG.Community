using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<PagedResult<Post>> GetFeedPaginatedAsync(PagedRequest request);
    }
}
