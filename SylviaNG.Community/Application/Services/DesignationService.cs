using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Designations.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _designationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DesignationService(IDesignationRepository designationRepository, IUnitOfWork unitOfWork)
        {
            _designationRepository = designationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(DesignationCreateRequest request)
        {
            var exists = await _designationRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Designation", "Name", request.Name);

            var entity = request.ToEntity();
            await _designationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.DesignationId;
        }

        public async Task UpdateAsync(long designationId, DesignationUpdateRequest request)
        {
            var entity = await _designationRepository.GetByIdAsync(designationId)
                ?? throw new NotFoundException("Designation", designationId);

            entity.ApplyUpdate(request);
            _designationRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long designationId)
        {
            var entity = await _designationRepository.GetByIdAsync(designationId)
                ?? throw new NotFoundException("Designation", designationId);

            _designationRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<DesignationResponse> GetByIdAsync(long designationId)
        {
            var entity = await _designationRepository.GetByIdAsync(designationId)
                ?? throw new NotFoundException("Designation", designationId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<DesignationResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _designationRepository.GetPaginatedAsync(request);

            return new PagedResult<DesignationResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
