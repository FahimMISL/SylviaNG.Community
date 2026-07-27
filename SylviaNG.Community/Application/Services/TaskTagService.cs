using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class TaskTagService : ITaskTagService
    {
        private readonly ITaskTagRepository _taskTagRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TaskTagService(ITaskTagRepository taskTagRepository, IUnitOfWork unitOfWork)
        {
            _taskTagRepository = taskTagRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(TaskTagCreateRequest request)
        {
            var exists = await _taskTagRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("TaskTag", "Name", request.Name);

            var entity = request.ToEntity();
            await _taskTagRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.TagId;
        }

        public async Task DeleteAsync(long tagId)
        {
            var entity = await _taskTagRepository.GetByIdAsync(tagId)
                ?? throw new NotFoundException("TaskTag", tagId);

            _taskTagRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TaskTagResponse> GetByIdAsync(long tagId)
        {
            var entity = await _taskTagRepository.GetByIdAsync(tagId)
                ?? throw new NotFoundException("TaskTag", tagId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<TaskTagResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _taskTagRepository.GetPaginatedAsync(request);

            return new PagedResult<TaskTagResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
