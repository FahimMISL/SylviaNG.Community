using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatMessages.Queries.ChatMessageGetAttachmentsPaged
{
    public class ChatMessageGetAttachmentsPagedHandler : IRequestHandler<ChatMessageGetAttachmentsPagedQuery, PagedResult<ChatMessageAttachmentGalleryItemResponse>>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageGetAttachmentsPagedHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<PagedResult<ChatMessageAttachmentGalleryItemResponse>> Handle(ChatMessageGetAttachmentsPagedQuery query, CancellationToken cancellationToken)
        {
            return await _chatMessageService.GetMediaAndFilesPagedAsync(query.ChatConversationId, query.CallerEmployeeId, query.Request);
        }
    }
}
