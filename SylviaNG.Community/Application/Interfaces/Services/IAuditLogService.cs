using SylviaNG.Community.Application.Features.AuditLogs.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IAuditLogService
    {
        /// <summary>
        /// Records an audit log entry. Called inline by other code that performs a data
        /// change - there is no public "create a log entry" REST endpoint.
        /// </summary>
        Task<long> LogAsync(AuditLogCreateRequest request);
        Task<PagedResult<AuditLogResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
