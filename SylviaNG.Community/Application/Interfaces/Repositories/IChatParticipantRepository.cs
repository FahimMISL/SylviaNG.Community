using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IChatParticipantRepository : IRepository<ChatParticipant>
    {
        Task<List<ChatParticipant>> GetActiveByConversationIdAsync(long conversationId);
        Task<ChatParticipant?> GetActiveAsync(long conversationId, long employeeId);
        Task<bool> IsActiveParticipantAsync(long conversationId, long employeeId);

        /// <summary>Ignores LeftAt - finds a participant row regardless of active/left status, so a re-add can reactivate the existing row instead of inserting a duplicate.</summary>
        Task<ChatParticipant?> GetAsync(long conversationId, long employeeId);
    }
}
