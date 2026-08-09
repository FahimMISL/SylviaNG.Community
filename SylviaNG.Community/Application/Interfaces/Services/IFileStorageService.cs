using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<long> CreateAsync(FileStorageCreateRequest request);
        Task<FileStorageResponse> GetByIdAsync(long fileId);
        Task<PagedResult<FileStorageResponse>> GetPaginatedAsync(PagedRequest request, string? module, long? entityId);
    }
}
