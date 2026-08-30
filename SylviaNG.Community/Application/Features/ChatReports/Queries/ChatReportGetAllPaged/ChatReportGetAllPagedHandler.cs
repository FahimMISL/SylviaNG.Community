using MediatR;
using SylviaNG.Community.Application.Features.ChatReports.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportGetAllPaged
{
    public class ChatReportGetAllPagedHandler : IRequestHandler<ChatReportGetAllPagedQuery, PagedResult<ChatReportQueueItemResponse>>
    {
        private readonly IChatReportService _chatReportService;

        public ChatReportGetAllPagedHandler(IChatReportService chatReportService)
        {
            _chatReportService = chatReportService;
        }

        public async Task<PagedResult<ChatReportQueueItemResponse>> Handle(ChatReportGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _chatReportService.GetPaginatedAsync(query.Request);
        }
    }
}
