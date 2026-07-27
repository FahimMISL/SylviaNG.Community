using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface INotificationPreferenceRepository : IRepository<NotificationPreference>
    {
        Task<NotificationPreference?> GetAsync(long employeeId, string category);
        Task<List<NotificationPreference>> GetByEmployeeAsync(long employeeId);
    }
}
