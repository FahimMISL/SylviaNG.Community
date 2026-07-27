using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class DashboardPreferenceRepository : Repository<DashboardPreference>, IDashboardPreferenceRepository
    {
        public DashboardPreferenceRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<DashboardPreference?> GetAsync(long employeeId, string widgetName)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.EmployeeId == employeeId && d.WidgetName == widgetName);
        }

        public async Task<List<DashboardPreference>> GetByEmployeeAsync(long employeeId)
        {
            return await _dbSet.Where(d => d.EmployeeId == employeeId).OrderBy(d => d.DisplayOrder).ToListAsync();
        }
    }
}
