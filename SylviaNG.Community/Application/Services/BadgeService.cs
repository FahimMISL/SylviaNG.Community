using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Badges.Models;
using SylviaNG.Community.Application.Features.EmployeeBadges.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IEmployeeBadgeRepository _employeeBadgeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BadgeService(
            IBadgeRepository badgeRepository,
            IEmployeeBadgeRepository employeeBadgeRepository,
            IUnitOfWork unitOfWork)
        {
            _badgeRepository = badgeRepository;
            _employeeBadgeRepository = employeeBadgeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(BadgeCreateRequest request)
        {
            var exists = await _badgeRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Badge", "Name", request.Name);

            var entity = request.ToEntity();
            await _badgeRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.BadgeId;
        }

        public async Task UpdateAsync(long badgeId, BadgeUpdateRequest request)
        {
            var entity = await _badgeRepository.GetByIdAsync(badgeId)
                ?? throw new NotFoundException("Badge", badgeId);

            if (request.Name != null)
            {
                var exists = await _badgeRepository.ExistsByNameAsync(request.Name, badgeId);
                if (exists)
                    throw new DuplicateException("Badge", "Name", request.Name);
            }

            entity.ApplyUpdate(request);
            _badgeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long badgeId)
        {
            var entity = await _badgeRepository.GetByIdAsync(badgeId)
                ?? throw new NotFoundException("Badge", badgeId);

            _badgeRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BadgeResponse> GetByIdAsync(long badgeId)
        {
            var entity = await _badgeRepository.GetByIdAsync(badgeId)
                ?? throw new NotFoundException("Badge", badgeId);

            return entity.ToResponse();
        }

        public async Task<List<BadgeResponse>> GetAllAsync()
        {
            var entities = await _badgeRepository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<PagedResult<BadgeResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _badgeRepository.GetPaginatedAsync(request);

            return new PagedResult<BadgeResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<long> AwardToEmployeeAsync(long employeeId, EmployeeBadgeAwardRequest request)
        {
            _ = await _badgeRepository.GetByIdAsync(request.BadgeId)
                ?? throw new NotFoundException("Badge", request.BadgeId);

            var entity = request.ToEntity(employeeId);
            await _employeeBadgeRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.EmployeeBadgeId;
        }

        public async Task<List<EmployeeBadgeResponse>> GetEmployeeBadgesAsync(long employeeId)
        {
            var entities = await _employeeBadgeRepository.GetByEmployeeIdAsync(employeeId);
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
