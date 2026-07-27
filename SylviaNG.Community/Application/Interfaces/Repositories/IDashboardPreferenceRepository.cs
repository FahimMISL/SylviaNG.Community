using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IDashboardPreferenceRepository : IRepository<DashboardPreference>
    {
        Task<DashboardPreference?> GetAsync(long employeeId, string widgetName);
        Task<List<DashboardPreference>> GetByEmployeeAsync(long employeeId);
    }
}
