using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.DashboardPreferences.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Services
{
    public class DashboardPreferenceService : IDashboardPreferenceService
    {
        private readonly IDashboardPreferenceRepository _dashboardPreferenceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DashboardPreferenceService(
            IDashboardPreferenceRepository dashboardPreferenceRepository,
            IUnitOfWork unitOfWork)
        {
            _dashboardPreferenceRepository = dashboardPreferenceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> UpsertAsync(DashboardPreferenceUpsertRequest request)
        {
            var existing = await _dashboardPreferenceRepository.GetAsync(request.EmployeeId, request.WidgetName);

            if (existing != null)
            {
                existing.ApplyUpdate(request);
                _dashboardPreferenceRepository.Update(existing);
                await _unitOfWork.SaveChangesAsync();
                return existing.PreferenceId;
            }

            var entity = request.ToEntity();
            await _dashboardPreferenceRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.PreferenceId;
        }

        public async Task DeleteAsync(long preferenceId)
        {
            var entity = await _dashboardPreferenceRepository.GetByIdAsync(preferenceId)
                ?? throw new NotFoundException("DashboardPreference", preferenceId);

            _dashboardPreferenceRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<DashboardPreferenceResponse>> GetByEmployeeAsync(long employeeId)
        {
            var preferences = await _dashboardPreferenceRepository.GetByEmployeeAsync(employeeId);
            return preferences.Select(p => p.ToResponse()).ToList();
        }
    }
}
