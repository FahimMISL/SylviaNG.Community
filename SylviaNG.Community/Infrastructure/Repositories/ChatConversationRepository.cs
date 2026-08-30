using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ChatConversationRepository : Repository<ChatConversation>, IChatConversationRepository
    {
        public ChatConversationRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<ChatConversation?> GetDirectConversationAsync(long employeeIdA, long employeeIdB)
        {
            var conversationIdsForA = _dbContext.Set<ChatParticipant>()
                .Where(p => p.EmployeeId == employeeIdA && p.LeftAt == null)
                .Select(p => p.ChatConversationId);

            return await _dbSet
                .Where(c => c.Type == ConversationTypeEnum.Direct && conversationIdsForA.Contains(c.ChatConversationId))
                .Where(c => _dbContext.Set<ChatParticipant>()
                    .Any(p => p.ChatConversationId == c.ChatConversationId && p.EmployeeId == employeeIdB && p.LeftAt == null))
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<ChatConversation>> GetMyConversationsPagedAsync(long employeeId, PagedRequest request)
        {
            var myConversationIds = _dbContext.Set<ChatParticipant>()
                .Where(p => p.EmployeeId == employeeId && p.LeftAt == null)
                .Select(p => p.ChatConversationId);

            // Pinned-first, then most recent activity - not a plain property sort, so this
            // bypasses the generic ToPaginatedResultAsync helper (built for SortBy-string
            // sorting) and paginates manually instead.
            var query = _dbSet
                .Where(c => myConversationIds.Contains(c.ChatConversationId))
                .OrderByDescending(c => _dbContext.Set<ChatParticipant>()
                    .Where(p => p.ChatConversationId == c.ChatConversationId && p.EmployeeId == employeeId)
                    .Select(p => p.IsPinned)
                    .FirstOrDefault())
                .ThenByDescending(c => c.LastMessageAt)
                .ThenByDescending(c => c.ChatConversationId);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResult<ChatConversation>
            {
                Data = items,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
