using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.EmployeeInterests.Models;
using SylviaNG.Community.Application.Features.Interests.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class InterestService : IInterestService
    {
        private readonly IInterestRepository _interestRepository;
        private readonly IEmployeeInterestRepository _employeeInterestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InterestService(
            IInterestRepository interestRepository,
            IEmployeeInterestRepository employeeInterestRepository,
            IUnitOfWork unitOfWork)
        {
            _interestRepository = interestRepository;
            _employeeInterestRepository = employeeInterestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(InterestCreateRequest request)
        {
            var exists = await _interestRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Interest", "Name", request.Name);

            var entity = request.ToEntity();
            await _interestRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.InterestId;
        }

        public async Task DeleteAsync(long interestId)
        {
            var entity = await _interestRepository.GetByIdAsync(interestId)
                ?? throw new NotFoundException("Interest", interestId);

            _interestRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<InterestResponse> GetByIdAsync(long interestId)
        {
            var entity = await _interestRepository.GetByIdAsync(interestId)
                ?? throw new NotFoundException("Interest", interestId);

            return entity.ToResponse();
        }

        public async Task<List<InterestResponse>> GetAllAsync()
        {
            var entities = await _interestRepository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<PagedResult<InterestResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _interestRepository.GetPaginatedAsync(request);

            return new PagedResult<InterestResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<long> AssignToEmployeeAsync(long employeeId, EmployeeInterestAssignRequest request)
        {
            _ = await _interestRepository.GetByIdAsync(request.InterestId)
                ?? throw new NotFoundException("Interest", request.InterestId);

            var alreadyAssigned = await _employeeInterestRepository.ExistsAsync(employeeId, request.InterestId);
            if (alreadyAssigned)
                throw new DuplicateException("EmployeeInterest", "InterestId", request.InterestId.ToString());

            var entity = request.ToEntity(employeeId);
            await _employeeInterestRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.EmployeeInterestId;
        }

        public async Task RemoveFromEmployeeAsync(long employeeId, long interestId)
        {
            var entity = await _employeeInterestRepository.GetAsync(employeeId, interestId)
                ?? throw new NotFoundException("EmployeeInterest", interestId);

            _employeeInterestRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<EmployeeInterestResponse>> GetEmployeeInterestsAsync(long employeeId)
        {
            var entities = await _employeeInterestRepository.GetByEmployeeIdAsync(employeeId);
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
