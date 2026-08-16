using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IConversationRepository : IRepository<Conversation>
    {
        Task<PagedResult<Conversation>> GetPaginatedForEmployeeAsync(long employeeId, PagedRequest request);
        Task<List<Conversation>> GetByListingIdAsync(long listingId);
    }
}
