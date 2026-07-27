using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.ContentReports.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class ContentReportService : IContentReportService
    {
        private readonly IContentReportRepository _contentReportRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContentReportService(
            IContentReportRepository contentReportRepository,
            IPostRepository postRepository,
            IUnitOfWork unitOfWork)
        {
            _contentReportRepository = contentReportRepository;
            _postRepository = postRepository;
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

        public async Task<PagedResult<ContentReportResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _contentReportRepository.GetPaginatedAsync(request);

            return new PagedResult<ContentReportResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
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
