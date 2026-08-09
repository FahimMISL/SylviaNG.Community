using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Roles.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(RoleCreateRequest request)
        {
            var exists = await _roleRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Role", "Name", request.Name);

            var entity = request.ToEntity();
            await _roleRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.RoleId;
        }

        public async Task UpdateAsync(long roleId, RoleUpdateRequest request)
        {
            var entity = await _roleRepository.GetByIdAsync(roleId)
                ?? throw new NotFoundException("Role", roleId);

            entity.ApplyUpdate(request);
            _roleRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long roleId)
        {
            var entity = await _roleRepository.GetByIdAsync(roleId)
                ?? throw new NotFoundException("Role", roleId);

            _roleRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<RoleResponse> GetByIdAsync(long roleId)
        {
            var entity = await _roleRepository.GetByIdAsync(roleId)
                ?? throw new NotFoundException("Role", roleId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<RoleResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _roleRepository.GetPaginatedAsync(request);

            return new PagedResult<RoleResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
