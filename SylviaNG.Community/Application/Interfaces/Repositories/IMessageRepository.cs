using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<List<Message>> GetByConversationIdAsync(long conversationId);
    }
}
