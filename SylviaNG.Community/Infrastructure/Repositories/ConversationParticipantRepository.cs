using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ConversationParticipantRepository : Repository<ConversationParticipant>, IConversationParticipantRepository
    {
        public ConversationParticipantRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(long conversationId, long employeeId)
        {
            return await _dbSet.AnyAsync(cp => cp.ConversationId == conversationId && cp.EmployeeId == employeeId);
        }

        public async Task<List<ConversationParticipant>> GetByConversationIdAsync(long conversationId)
        {
            return await _dbSet.Where(cp => cp.ConversationId == conversationId).ToListAsync();
        }
    }
}
