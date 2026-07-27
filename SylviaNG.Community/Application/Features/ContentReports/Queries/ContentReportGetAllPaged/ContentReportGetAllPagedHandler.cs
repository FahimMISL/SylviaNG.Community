using MediatR;
using SylviaNG.Community.Application.Features.ContentReports.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ContentReports.Queries.ContentReportGetAllPaged
{
    public class ContentReportGetAllPagedHandler : IRequestHandler<ContentReportGetAllPagedQuery, PagedResult<ContentReportResponse>>
    {
        private readonly IContentReportService _contentReportService;

        public ContentReportGetAllPagedHandler(IContentReportService contentReportService)
        {
            _contentReportService = contentReportService;
        }

        public async Task<PagedResult<ContentReportResponse>> Handle(ContentReportGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _contentReportService.GetPaginatedAsync(query.Request);
        }
    }
}
