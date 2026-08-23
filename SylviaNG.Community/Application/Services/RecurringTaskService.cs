using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class RecurringTaskService : IRecurringTaskService
    {
        private readonly IRecurringTaskRepository _recurringTaskRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RecurringTaskService(IRecurringTaskRepository recurringTaskRepository, IUnitOfWork unitOfWork)
        {
            _recurringTaskRepository = recurringTaskRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(RecurringTaskCreateRequest request)
        {
            var entity = request.ToEntity();
            await _recurringTaskRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.RecurringTaskId;
        }

        public async Task UpdateAsync(long recurringTaskId, RecurringTaskUpdateRequest request)
        {
            var entity = await _recurringTaskRepository.GetByIdAsync(recurringTaskId)
                ?? throw new NotFoundException("RecurringTask", recurringTaskId);

            entity.ApplyUpdate(request);
            _recurringTaskRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>US-7.12: "cancel the series" deactivates rather than hard-deletes, so already-generated
        /// Task rows (which point back via Task.RecurringTaskId) are unaffected - only future generation stops.</summary>
        public async Task DeleteAsync(long recurringTaskId)
        {
            var entity = await _recurringTaskRepository.GetByIdAsync(recurringTaskId)
                ?? throw new NotFoundException("RecurringTask", recurringTaskId);

            entity.IsActive = false;
            _recurringTaskRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<RecurringTaskResponse> GetByIdAsync(long recurringTaskId)
        {
            var entity = await _recurringTaskRepository.GetByIdAsync(recurringTaskId)
                ?? throw new NotFoundException("RecurringTask", recurringTaskId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<RecurringTaskResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _recurringTaskRepository.GetPaginatedAsync(request);

            return new PagedResult<RecurringTaskResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
