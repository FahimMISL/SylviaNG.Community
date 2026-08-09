using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        /// <summary>
        /// Feeds back only posts visible to the caller: Everyone-visibility posts, plus
        /// Department/Branch-visibility posts whose author shares the caller's
        /// DepartmentId/SiteId respectively. callerDepartmentId/callerSiteId are null-safe -
        /// a caller with no Department/Site on record simply won't match those scopes.
        /// </summary>
        Task<PagedResult<Post>> GetFeedPaginatedAsync(PostFilterRequest request, long? callerDepartmentId, long? callerSiteId);

        /// <summary>
        /// Posts scoped to a single group (its own feed page). No visibility/membership
        /// filtering here - the caller (GroupService) checks membership before calling this.
        /// </summary>
        Task<PagedResult<Post>> GetGroupFeedPaginatedAsync(PostFilterRequest request, long groupId);
    }
}
