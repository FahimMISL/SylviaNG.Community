using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ChatMessageRepository : Repository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<ChatMessage>> GetByConversationPagedAsync(long conversationId, PagedRequest request)
        {
            var query = _dbSet
                .Where(m => m.ChatConversationId == conversationId)
                .OrderByDescending(m => m.SentAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResult<ChatMessage>
            {
                Data = items,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<int> GetUnreadCountAsync(long conversationId, long employeeId, DateTime? lastReadAt)
        {
            return await _dbSet.CountAsync(m =>
                m.ChatConversationId == conversationId &&
                m.SenderEmployeeId != employeeId &&
                (lastReadAt == null || m.SentAt > lastReadAt));
        }

        public async Task<PagedResult<ChatMessage>> SearchAsync(long employeeId, string searchTerm, PagedRequest request)
        {
            var myConversationIds = _dbContext.Set<ChatParticipant>()
                .Where(p => p.EmployeeId == employeeId && p.LeftAt == null)
                .Select(p => p.ChatConversationId);

            var query = _dbSet
                .Where(m => myConversationIds.Contains(m.ChatConversationId) && m.Body != null && m.Body.Contains(searchTerm))
                .OrderByDescending(m => m.SentAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResult<ChatMessage>
            {
                Data = items,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
