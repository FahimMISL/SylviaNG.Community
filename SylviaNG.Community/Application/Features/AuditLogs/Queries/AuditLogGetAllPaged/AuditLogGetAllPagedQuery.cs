using MediatR;
using SylviaNG.Community.Application.Features.AuditLogs.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.AuditLogs.Queries.AuditLogGetAllPaged
{
    public class AuditLogGetAllPagedQuery : IRequest<PagedResult<AuditLogResponse>>
    {
        public PagedRequest Request { get; set; }

        public AuditLogGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
