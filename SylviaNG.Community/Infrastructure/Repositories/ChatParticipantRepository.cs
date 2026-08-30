using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ChatParticipantRepository : Repository<ChatParticipant>, IChatParticipantRepository
    {
        public ChatParticipantRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<ChatParticipant>> GetActiveByConversationIdAsync(long conversationId)
        {
            return await _dbSet
                .Where(p => p.ChatConversationId == conversationId && p.LeftAt == null)
                .ToListAsync();
        }

        public async Task<ChatParticipant?> GetActiveAsync(long conversationId, long employeeId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.ChatConversationId == conversationId && p.EmployeeId == employeeId && p.LeftAt == null);
        }

        public async Task<bool> IsActiveParticipantAsync(long conversationId, long employeeId)
        {
            return await _dbSet
                .AnyAsync(p => p.ChatConversationId == conversationId && p.EmployeeId == employeeId && p.LeftAt == null);
        }
    }
}
