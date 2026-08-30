using MediatR;
using SylviaNG.Community.Application.Features.ChatReports.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportGetAllPaged
{
    public class ChatReportGetAllPagedQuery : IRequest<PagedResult<ChatReportQueueItemResponse>>
    {
        public PagedRequest Request { get; set; }

        public ChatReportGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
