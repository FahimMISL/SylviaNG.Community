using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IChatMessageRepository : IRepository<ChatMessage>
    {
        Task<PagedResult<ChatMessage>> GetByConversationPagedAsync(long conversationId, PagedRequest request);
        Task<int> GetUnreadCountAsync(long conversationId, long employeeId, DateTime? lastReadAt);

        /// <summary>Body-text search scoped to conversations the employee is an active participant of.</summary>
        Task<PagedResult<ChatMessage>> SearchAsync(long employeeId, string searchTerm, PagedRequest request);
    }
}
