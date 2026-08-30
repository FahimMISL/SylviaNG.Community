using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ChatMessageReactionRepository : Repository<ChatMessageReaction>, IChatMessageReactionRepository
    {
        public ChatMessageReactionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<ChatMessageReaction?> GetAsync(long chatMessageId, long employeeId)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.ChatMessageId == chatMessageId && r.EmployeeId == employeeId);
        }

        public async Task<List<ChatMessageReaction>> GetByMessageIdsAsync(IEnumerable<long> chatMessageIds)
        {
            return await _dbSet
                .Where(r => chatMessageIds.Contains(r.ChatMessageId))
                .ToListAsync();
        }
    }
}
