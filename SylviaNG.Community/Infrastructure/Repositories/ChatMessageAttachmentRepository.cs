using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ChatMessageAttachmentRepository : Repository<ChatMessageAttachment>, IChatMessageAttachmentRepository
    {
        public ChatMessageAttachmentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<ChatMessageAttachment>> GetByMessageIdsAsync(IEnumerable<long> chatMessageIds)
        {
            return await _dbSet
                .Where(a => chatMessageIds.Contains(a.ChatMessageId))
                .ToListAsync();
        }

        public async Task<PagedResult<ChatMessageAttachment>> GetByConversationPagedAsync(long conversationId, PagedRequest request)
        {
            var query = _dbSet
                .Where(a => _dbContext.Set<ChatMessage>().Any(m =>
                    m.ChatMessageId == a.ChatMessageId && m.ChatConversationId == conversationId && !m.IsDeleted))
                .OrderByDescending(a => a.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResult<ChatMessageAttachment>
            {
                Data = items,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
