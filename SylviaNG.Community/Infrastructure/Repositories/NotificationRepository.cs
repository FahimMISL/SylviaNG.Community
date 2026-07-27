using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Notification>> GetPaginatedByEmployeeAsync(long employeeId, PagedRequest request)
        {
            var query = _dbSet.Where(n => n.EmployeeId == employeeId).AsQueryable();

            request.SearchProperties ??= new[] { nameof(Notification.Title), nameof(Notification.Message) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
