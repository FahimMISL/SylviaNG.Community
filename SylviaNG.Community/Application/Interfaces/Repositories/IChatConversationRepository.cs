using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IChatConversationRepository : IRepository<ChatConversation>
    {
        Task<ChatConversation?> GetDirectConversationAsync(long employeeIdA, long employeeIdB);
        Task<PagedResult<ChatConversation>> GetMyConversationsPagedAsync(long employeeId, PagedRequest request);
    }
}
