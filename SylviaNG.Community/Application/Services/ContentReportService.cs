using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.ContentReports.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Services
{
    public class ContentReportService : IContentReportService
    {
        private const int ContentPreviewMaxLength = 200;

        private readonly IContentReportRepository _contentReportRepository;
        private readonly IPostRepository _postRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContentReportService(
            IContentReportRepository contentReportRepository,
            IPostRepository postRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _contentReportRepository = contentReportRepository;
            _postRepository = postRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ContentReportCreateRequest request)
        {
            _ = await _postRepository.GetByIdAsync(request.PostId)
                ?? throw new NotFoundException("Post", request.PostId);

            var entity = request.ToEntity();
            await _contentReportRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ReportId;
        }

        public async Task<PagedResult<ContentReportQueueItemResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _contentReportRepository.GetPaginatedAsync(request);

            var items = new List<ContentReportQueueItemResponse>();
            foreach (var report in pagedResult.Data)
            {
                items.Add(await EnrichAsync(report));
            }

            return new PagedResult<ContentReportQueueItemResponse>
            {
                Data = items,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        private async Task<ContentReportQueueItemResponse> EnrichAsync(ContentReport report)
        {
            var post = await _postRepository.GetByIdAsync(report.PostId);
            var reporter = await _employeeRepository.GetByIdAsync(report.ReportedBy);
            var author = post != null ? await _employeeRepository.GetByIdAsync(post.EmployeeId) : null;

            return new ContentReportQueueItemResponse
            {
                ReportId = report.ReportId,
                ReportedBy = report.ReportedBy,
                ReporterName = reporter?.EmployeeName ?? "Unknown",
                PostId = report.PostId,
                PostContentPreview = Truncate(post?.Content, ContentPreviewMaxLength) ?? "[post deleted]",
                PostType = post?.Type ?? string.Empty,
                PostAuthorId = post?.EmployeeId ?? 0,
                PostAuthorName = author?.EmployeeName ?? "Unknown",
                IsPostHidden = post?.IsHidden ?? false,
                IsPostLocked = post?.IsLocked ?? false,
                Reason = report.Reason,
                Status = report.Status,
                ReviewedBy = report.ReviewedBy,
                ReviewedAt = report.ReviewedAt,
                CreatedAt = report.CreatedAt
            };
        }

        private static string? Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value[..maxLength] + "...";
        }

        public async Task ResolveAsync(long reportId, ContentReportResolveRequest request)
        {
            var entity = await _contentReportRepository.GetByIdAsync(reportId)
                ?? throw new NotFoundException("ContentReport", reportId);

            entity.Status = request.Status;
            entity.ReviewedBy = request.ReviewedBy;
            entity.ReviewedAt = DateTime.UtcNow;

            _contentReportRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
