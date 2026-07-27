using SylviaNG.Community.Application.Features.AuditLogs.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    /// <summary>
    /// AuditLog is an insert-only log table: LogAsync is called inline by other code
    /// that performs a data change (there is no public "create a log entry" REST endpoint).
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuditLogService(IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork)
        {
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> LogAsync(AuditLogCreateRequest request)
        {
            var entity = request.ToEntity();
            await _auditLogRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.AuditId;
        }

        public async Task<PagedResult<AuditLogResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _auditLogRepository.GetPaginatedAsync(request);

            return new PagedResult<AuditLogResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
