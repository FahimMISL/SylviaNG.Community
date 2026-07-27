using SylviaNG.Community.Application.Features.ContentReports.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IContentReportService
    {
        Task<long> CreateAsync(ContentReportCreateRequest request);
        Task<PagedResult<ContentReportResponse>> GetPaginatedAsync(PagedRequest request);
        Task ResolveAsync(long reportId, ContentReportResolveRequest request);
    }
}
