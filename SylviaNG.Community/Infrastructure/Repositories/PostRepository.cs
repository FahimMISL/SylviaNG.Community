using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Post>> GetFeedPaginatedAsync(PostFilterRequest request, long callerEmployeeId, long? callerDepartmentId, long? callerSiteId)
        {
            var query = from p in _dbSet.Include(p => p.Group)
                        join e in _dbContext.Set<Employee>() on p.EmployeeId equals e.EmployeeId
                        where !p.IsHidden
                            && (p.GroupId == null
                                ? (p.Visibility == VisibilityEnum.Everyone
                                    || (p.Visibility == VisibilityEnum.Department && callerDepartmentId != null && e.DepartmentId == callerDepartmentId)
                                    || (p.Visibility == VisibilityEnum.Branch && callerSiteId != null && e.SiteId == callerSiteId))
                                : _dbContext.Set<GroupMember>().Any(gm => gm.GroupId == p.GroupId && gm.EmployeeId == callerEmployeeId && gm.IsActive))
                        select p;

            if (request.IsAnnouncement.HasValue)
                query = query.Where(p => p.IsAnnouncement == request.IsAnnouncement.Value);

            if (request.IsPoll.HasValue)
                query = query.Where(p => p.IsPoll == request.IsPoll.Value);

            if (request.EmployeeId.HasValue)
                query = query.Where(p => p.EmployeeId == request.EmployeeId.Value);

            request.SearchProperties ??= new[] { nameof(Post.Content) };
            request.SortBy ??= nameof(Post.CreatedAt);
            request.SortDirection ??= "desc";

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<PagedResult<Post>> GetGroupFeedPaginatedAsync(PostFilterRequest request, long groupId)
        {
            var query = _dbSet.Include(p => p.Group).Where(p => p.GroupId == groupId && !p.IsHidden);

            if (request.IsAnnouncement.HasValue)
                query = query.Where(p => p.IsAnnouncement == request.IsAnnouncement.Value);

            if (request.IsPoll.HasValue)
                query = query.Where(p => p.IsPoll == request.IsPoll.Value);

            if (request.EmployeeId.HasValue)
                query = query.Where(p => p.EmployeeId == request.EmployeeId.Value);

            request.SearchProperties ??= new[] { nameof(Post.Content) };
            request.SortBy ??= nameof(Post.CreatedAt);
            request.SortDirection ??= "desc";

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
