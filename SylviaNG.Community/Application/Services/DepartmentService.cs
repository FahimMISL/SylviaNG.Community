using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Departments.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(DepartmentCreateRequest request)
        {
            var exists = await _departmentRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Department", "Name", request.Name);

            var entity = request.ToEntity();
            await _departmentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.DepartmentId;
        }

        public async Task UpdateAsync(long departmentId, DepartmentUpdateRequest request)
        {
            var entity = await _departmentRepository.GetByIdAsync(departmentId)
                ?? throw new NotFoundException("Department", departmentId);

            entity.ApplyUpdate(request);
            _departmentRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long departmentId)
        {
            var entity = await _departmentRepository.GetByIdAsync(departmentId)
                ?? throw new NotFoundException("Department", departmentId);

            _departmentRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<DepartmentResponse> GetByIdAsync(long departmentId)
        {
            var entity = await _departmentRepository.GetByIdAsync(departmentId)
                ?? throw new NotFoundException("Department", departmentId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<DepartmentResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _departmentRepository.GetPaginatedAsync(request);

            return new PagedResult<DepartmentResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
