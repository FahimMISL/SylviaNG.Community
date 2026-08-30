using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IChatMessageReactionRepository : IRepository<ChatMessageReaction>
    {
        Task<ChatMessageReaction?> GetAsync(long chatMessageId, long employeeId);
        Task<List<ChatMessageReaction>> GetByMessageIdsAsync(IEnumerable<long> chatMessageIds);
    }
}
