using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IFileStorageRepository : IRepository<FileStorage>
    {
        Task<PagedResult<FileStorage>> GetPaginatedAsync(PagedRequest request, string? module, long? entityId);
    }
}
