using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class NotificationPreferenceRepository : Repository<NotificationPreference>, INotificationPreferenceRepository
    {
        public NotificationPreferenceRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<NotificationPreference?> GetAsync(long employeeId, string category)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.EmployeeId == employeeId && p.Category == category);
        }

        public async Task<List<NotificationPreference>> GetByEmployeeAsync(long employeeId)
        {
            return await _dbSet.Where(p => p.EmployeeId == employeeId).ToListAsync();
        }
    }
}
