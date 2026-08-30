using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatMessages.Queries.ChatMessageSearch
{
    public class ChatMessageSearchQuery : IRequest<PagedResult<ChatMessageResponse>>
    {
        public string SearchTerm { get; set; }
        public PagedRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatMessageSearchQuery(string searchTerm, PagedRequest request, long callerEmployeeId)
        {
            SearchTerm = searchTerm;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
