using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class FileStorageRepository : Repository<FileStorage>, IFileStorageRepository
    {
        public FileStorageRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<FileStorage>> GetPaginatedAsync(PagedRequest request, string? module, long? entityId)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(module))
                query = query.Where(f => f.Module == module);

            if (entityId.HasValue)
                query = query.Where(f => f.EntityId == entityId.Value);

            request.SearchProperties ??= new[] { nameof(FileStorage.FileName), nameof(FileStorage.OriginalFileName) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
