using MediatR;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatConversations.Queries.ChatConversationGetAllPaged
{
    public class ChatConversationGetAllPagedQuery : IRequest<PagedResult<ChatConversationSummaryResponse>>
    {
        public PagedRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatConversationGetAllPagedQuery(PagedRequest request, long callerEmployeeId)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
