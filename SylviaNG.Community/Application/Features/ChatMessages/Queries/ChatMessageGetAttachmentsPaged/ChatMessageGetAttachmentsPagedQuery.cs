using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatMessages.Queries.ChatMessageGetAttachmentsPaged
{
    public class ChatMessageGetAttachmentsPagedQuery : IRequest<PagedResult<ChatMessageAttachmentGalleryItemResponse>>
    {
        public long ChatConversationId { get; set; }
        public PagedRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatMessageGetAttachmentsPagedQuery(long chatConversationId, PagedRequest request, long callerEmployeeId)
        {
            ChatConversationId = chatConversationId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
