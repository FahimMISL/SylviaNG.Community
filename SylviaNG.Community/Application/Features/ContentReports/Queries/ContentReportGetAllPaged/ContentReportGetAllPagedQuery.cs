using MediatR;
using SylviaNG.Community.Application.Features.ContentReports.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ContentReports.Queries.ContentReportGetAllPaged
{
    public class ContentReportGetAllPagedQuery : IRequest<PagedResult<ContentReportQueueItemResponse>>
    {
        public PagedRequest Request { get; set; }

        public ContentReportGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
