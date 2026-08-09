using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Branches.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BranchService(IBranchRepository branchRepository, IUnitOfWork unitOfWork)
        {
            _branchRepository = branchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(BranchCreateRequest request)
        {
            var exists = await _branchRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Branch", "Name", request.Name);

            var entity = request.ToEntity();
            await _branchRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.BranchId;
        }

        public async Task UpdateAsync(long branchId, BranchUpdateRequest request)
        {
            var entity = await _branchRepository.GetByIdAsync(branchId)
                ?? throw new NotFoundException("Branch", branchId);

            entity.ApplyUpdate(request);
            _branchRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long branchId)
        {
            var entity = await _branchRepository.GetByIdAsync(branchId)
                ?? throw new NotFoundException("Branch", branchId);

            _branchRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BranchResponse> GetByIdAsync(long branchId)
        {
            var entity = await _branchRepository.GetByIdAsync(branchId)
                ?? throw new NotFoundException("Branch", branchId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<BranchResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _branchRepository.GetPaginatedAsync(request);

            return new PagedResult<BranchResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
