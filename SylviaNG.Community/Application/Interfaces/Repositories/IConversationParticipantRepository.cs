using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IConversationParticipantRepository : IRepository<ConversationParticipant>
    {
        Task<bool> ExistsAsync(long conversationId, long employeeId);
        Task<List<ConversationParticipant>> GetByConversationIdAsync(long conversationId);
    }
}
