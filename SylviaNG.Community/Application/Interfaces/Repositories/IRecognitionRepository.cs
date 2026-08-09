using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IRecognitionRepository : IRepository<Recognition>
    {
        Task<PagedResult<Recognition>> GetPaginatedAsync(PagedRequest request, long? senderId = null, long? recipientId = null);
    }
}
