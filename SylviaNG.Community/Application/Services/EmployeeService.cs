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
        private readonly IEmployeeContactLinkRepository _employeeContactLinkRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICoreGrpcClient _coreGrpcClient;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IEmployeeContactLinkRepository employeeContactLinkRepository,
            IUnitOfWork unitOfWork,
            ICoreGrpcClient coreGrpcClient,
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _employeeContactLinkRepository = employeeContactLinkRepository;
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

            await DiffContactLinksAsync(employeeId, request.ContactLinks);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Bundle-diffs the submitted contact-links list against the employee's existing rows:
        /// items with a null Id are added, items with a non-null Id matching an existing row are
        /// updated in place, and existing rows whose Id is absent from the submitted list are
        /// removed. All changes are committed together by the single SaveChangesAsync call at
        /// the end of UpdateProfileAsync.
        /// </summary>
        private async System.Threading.Tasks.Task DiffContactLinksAsync(long employeeId, List<EmployeeContactLinkItem> submitted)
        {
            var existing = await _employeeContactLinkRepository.GetByEmployeeIdAsync(employeeId);
            var existingById = existing.ToDictionary(e => e.EmployeeContactLinkId);
            var submittedIds = submitted.Where(s => s.Id.HasValue).Select(s => s.Id!.Value).ToHashSet();

            var toRemove = existing.Where(e => !submittedIds.Contains(e.EmployeeContactLinkId)).ToList();
            if (toRemove.Count > 0)
                _employeeContactLinkRepository.DeleteRange(toRemove);

            foreach (var item in submitted.Where(s => s.Id.HasValue))
            {
                // A submitted Id not found here belongs to another employee (or was already
                // removed) - existingById is scoped to this employee's own rows, so this is an
                // implicit ownership guard: it can never mutate another employee's link.
                if (!existingById.TryGetValue(item.Id!.Value, out var row))
                    continue;

                row.Platform = item.Platform;
                row.Url = item.Url;
                row.Visibility = item.Visibility;
                _employeeContactLinkRepository.Update(row);
            }

            foreach (var item in submitted.Where(s => !s.Id.HasValue))
            {
                await _employeeContactLinkRepository.AddAsync(new EmployeeContactLink
                {
                    EmployeeId = employeeId,
                    Platform = item.Platform,
                    Url = item.Url,
                    Visibility = item.Visibility
                });
            }
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
            var contactLinks = await _employeeContactLinkRepository.GetByEmployeeIdAsync(employeeId);
            return entity.ToResponse(viewerEmployeeId, viewerIsHrAdmin, lookups, contactLinks);
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

            if (departmentIds.Count == 0 && designationIds.Count == 0 && siteIds.Count == 0)
                return new CoreBatchLookupResult();

            try
            {
                return await _coreGrpcClient.GetMasterDataAsync(departmentIds, designationIds, siteIds);
            }
            catch (Exception ex)
            {
                // Department/Designation/Site *names* are a display nicety resolved from
                // an external Core service - if it's unreachable, the directory/profile should
                // still work with raw IDs rather than failing the whole request.
                _logger.LogWarning(ex, "Failed to resolve master data names from CoreService; falling back to IDs only.");
                return new CoreBatchLookupResult();
            }
        }
    }
}
