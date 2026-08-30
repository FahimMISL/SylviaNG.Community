using SylviaNG.Community.Application.Features.ChatReports.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IChatReportService
    {
        Task<PagedResult<ChatReportQueueItemResponse>> GetPaginatedAsync(PagedRequest request);
        Task ResolveAsync(long reportId, ChatReportResolveRequest request);
    }
}
