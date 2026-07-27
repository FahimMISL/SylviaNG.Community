using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IFileStorageRepository _fileStorageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public FileStorageService(IFileStorageRepository fileStorageRepository, IUnitOfWork unitOfWork)
        {
            _fileStorageRepository = fileStorageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(FileStorageCreateRequest request)
        {
            var entity = request.ToEntity();
            await _fileStorageRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.FileId;
        }

        public async Task<FileStorageResponse> GetByIdAsync(long fileId)
        {
            var entity = await _fileStorageRepository.GetByIdAsync(fileId)
                ?? throw new NotFoundException("FileStorage", fileId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<FileStorageResponse>> GetPaginatedAsync(PagedRequest request, string? module, long? entityId)
        {
            var pagedResult = await _fileStorageRepository.GetPaginatedAsync(request, module, entityId);

            return new PagedResult<FileStorageResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
