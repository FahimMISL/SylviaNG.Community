using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IChatParticipantRepository : IRepository<ChatParticipant>
    {
        Task<List<ChatParticipant>> GetActiveByConversationIdAsync(long conversationId);
        Task<ChatParticipant?> GetActiveAsync(long conversationId, long employeeId);
        Task<bool> IsActiveParticipantAsync(long conversationId, long employeeId);
    }
}
