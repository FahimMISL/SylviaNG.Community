using Microsoft.Extensions.Logging;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Common.Models;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Interfaces.Externals;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICoreGrpcClient _coreGrpcClient;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            ICoreGrpcClient coreGrpcClient,
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _coreGrpcClient = coreGrpcClient;
            _logger = logger;
        }

        public async Task<long> CreateAsync(EmployeeCreateRequest request)
        {
            var exists = await _employeeRepository.ExistsByEmailAsync(request.Email);
            if (exists)
                throw new DuplicateException("Employee", "Email", request.Email);

            var entity = request.ToEntity();
            await _employeeRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // EmployeeCode depends on the DB-generated EmployeeId, so it's assigned post-insert.
            entity.EmployeeCode = $"EMP{entity.EmployeeId:D5}";
            _employeeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.EmployeeId;
        }

        public async System.Threading.Tasks.Task UpdateProfileAsync(long employeeId, EmployeeUpdateProfileRequest request, long? viewerEmployeeId)
        {
            if (viewerEmployeeId != employeeId)
                throw new ForbiddenException("You can only edit your own profile.");

            var entity = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new NotFoundException("Employee", employeeId);

            entity.ApplyProfileUpdate(request);
            _employeeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task UpdatePhotoAsync(long employeeId, string storagePath, long? viewerEmployeeId)
        {
            if (viewerEmployeeId != employeeId)
                throw new ForbiddenException("You can only edit your own profile.");

            var entity = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new NotFoundException("Employee", employeeId);

            entity.PhotoUrl = storagePath;
            _employeeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task UpdateCoverPhotoAsync(long employeeId, string storagePath, long? viewerEmployeeId)
        {
            if (viewerEmployeeId != employeeId)
                throw new ForbiddenException("You can only edit your own profile.");

            var entity = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new NotFoundException("Employee", employeeId);

            entity.CoverPhotoUrl = storagePath;
            _employeeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeactivateAsync(long employeeId)
        {
            var entity = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new NotFoundException("Employee", employeeId);

            entity.IsActive = false;
            _employeeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<EmployeeResponse> GetByIdAsync(long employeeId, long? viewerEmployeeId, bool viewerIsHrAdmin)
        {
            var entity = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new NotFoundException("Employee", employeeId);

            var lookups = await ResolveLookupsAsync(new[] { entity });
            return entity.ToResponse(viewerEmployeeId, viewerIsHrAdmin, lookups);
        }

        public async Task<PagedResult<EmployeeDirectoryCardResponse>> GetDirectoryPaginatedAsync(EmployeeFilterRequest request)
        {
            var paged = await _employeeRepository.GetPaginatedAsync(request, activeOnly: true);
            var lookups = await ResolveLookupsAsync(paged.Data);

            return new PagedResult<EmployeeDirectoryCardResponse>
            {
                Data = paged.Data.Select(e => e.ToDirectoryCard(lookups)).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }

        public async Task<PagedResult<EmployeeManagementRowResponse>> GetManagementPaginatedAsync(EmployeeFilterRequest request)
        {
            var paged = await _employeeRepository.GetPaginatedAsync(request, activeOnly: false);
            var lookups = await ResolveLookupsAsync(paged.Data);

            return new PagedResult<EmployeeManagementRowResponse>
            {
                Data = paged.Data.Select(e => e.ToManagementRow(lookups)).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }

        private async Task<CoreBatchLookupResult> ResolveLookupsAsync(IEnumerable<Employee> employees)
        {
            var departmentIds = employees.Where(e => e.DepartmentId.HasValue).Select(e => e.DepartmentId!.Value).Distinct().ToList();
            var designationIds = employees.Where(e => e.DesignatioId.HasValue).Select(e => e.DesignatioId!.Value).Distinct().ToList();
            var siteIds = employees.Where(e => e.SiteId.HasValue).Select(e => e.SiteId!.Value).Distinct().ToList();
            var gradeIds = employees.Where(e => e.GradeId.HasValue).Select(e => e.GradeId!.Value).Distinct().ToList();

            if (departmentIds.Count == 0 && designationIds.Count == 0 && siteIds.Count == 0 && gradeIds.Count == 0)
                return new CoreBatchLookupResult();

            try
            {
                return await _coreGrpcClient.GetMasterDataAsync(departmentIds, designationIds, gradeIds, siteIds);
            }
            catch (Exception ex)
            {
                // Department/Designation/Site/Grade *names* are a display nicety resolved from
                // an external Core service - if it's unreachable, the directory/profile should
                // still work with raw IDs rather than failing the whole request.
                _logger.LogWarning(ex, "Failed to resolve master data names from CoreService; falling back to IDs only.");
                return new CoreBatchLookupResult();
            }
        }
    }
}
