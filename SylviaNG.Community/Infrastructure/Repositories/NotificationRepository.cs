using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Features.Notifications.Models;
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

        public async Task<PagedResult<Notification>> GetPaginatedByEmployeeAsync(long employeeId, NotificationFilterRequest request)
        {
            var query = _dbSet.Where(n => n.EmployeeId == employeeId).AsQueryable();

            if (request.IsRead.HasValue)
                query = query.Where(n => n.IsRead == request.IsRead.Value);

            if (!string.IsNullOrEmpty(request.Category))
                query = query.Where(n => n.Category == request.Category);

            request.SearchProperties ??= new[] { nameof(Notification.Title), nameof(Notification.Message) };
            request.SortBy ??= nameof(Notification.CreatedAt);
            request.SortDirection ??= "desc";

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<int> GetUnreadCountAsync(long employeeId)
        {
            return await _dbSet.CountAsync(n => n.EmployeeId == employeeId && !n.IsRead);
        }

        public async Task<int> MarkAllAsReadAsync(long employeeId)
        {
            var unreadNotifications = await _dbSet
                .Where(n => n.EmployeeId == employeeId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                _dbSet.Update(notification);
            }

            return unreadNotifications.Count;
        }
    }
}
