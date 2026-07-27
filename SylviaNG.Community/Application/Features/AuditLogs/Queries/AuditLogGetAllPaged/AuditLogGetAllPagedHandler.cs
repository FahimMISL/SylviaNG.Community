using MediatR;
using SylviaNG.Community.Application.Features.AuditLogs.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.AuditLogs.Queries.AuditLogGetAllPaged
{
    public class AuditLogGetAllPagedHandler : IRequestHandler<AuditLogGetAllPagedQuery, PagedResult<AuditLogResponse>>
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogGetAllPagedHandler(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<PagedResult<AuditLogResponse>> Handle(AuditLogGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _auditLogService.GetPaginatedAsync(query.Request);
        }
    }
}
