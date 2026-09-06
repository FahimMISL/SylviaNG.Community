using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IChatMessageAttachmentRepository : IRepository<ChatMessageAttachment>
    {
        Task<List<ChatMessageAttachment>> GetByMessageIdsAsync(IEnumerable<long> chatMessageIds);

        /// <summary>Every non-deleted-message attachment in a conversation, newest first - backs the "Media and Files" gallery.</summary>
        Task<PagedResult<ChatMessageAttachment>> GetByConversationPagedAsync(long conversationId, PagedRequest request);
    }
}
