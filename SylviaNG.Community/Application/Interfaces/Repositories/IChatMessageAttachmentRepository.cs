using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IChatMessageAttachmentRepository : IRepository<ChatMessageAttachment>
    {
        Task<List<ChatMessageAttachment>> GetByMessageIdsAsync(IEnumerable<long> chatMessageIds);
    }
}
