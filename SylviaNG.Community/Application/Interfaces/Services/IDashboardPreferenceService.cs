using SylviaNG.Community.Application.Features.DashboardPreferences.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IDashboardPreferenceService
    {
        Task<long> UpsertAsync(DashboardPreferenceUpsertRequest request);
        Task DeleteAsync(long preferenceId);
        Task<List<DashboardPreferenceResponse>> GetByEmployeeAsync(long employeeId);
    }
}
