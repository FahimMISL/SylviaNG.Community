using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

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
    }
}
