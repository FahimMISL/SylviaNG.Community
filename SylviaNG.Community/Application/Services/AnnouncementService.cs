using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AnnouncementService(
            IAnnouncementRepository announcementRepository,
            IUnitOfWork unitOfWork)
        {
            _announcementRepository = announcementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(AnnouncementCreateRequest request)
        {
            var exists = await _announcementRepository.ExistsByTitleAndSiteIdAsync(request.Title, request.SiteId);
            if (exists)
                throw new DuplicateException("Announcement", "Title", request.Title);

            var entity = request.ToEntity();
            await _announcementRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.AnnouncementId;
        }

        public async Task UpdateAsync(long announcementId, AnnouncementUpdateRequest request)
        {
            var entity = await _announcementRepository.GetByIdAsync(announcementId)
                ?? throw new NotFoundException("Announcement", announcementId);

            entity.ApplyUpdate(request);
            _announcementRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long announcementId)
        {
            var entity = await _announcementRepository.GetByIdAsync(announcementId)
                ?? throw new NotFoundException("Announcement", announcementId);

            _announcementRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AnnouncementResponse> GetByIdAsync(long announcementId)
        {
            var entity = await _announcementRepository.GetByIdAsync(announcementId)
                ?? throw new NotFoundException("Announcement", announcementId);

            return entity.ToResponse();
        }

        public async Task<List<AnnouncementResponse>> GetAllAsync()
        {
            var entities = await _announcementRepository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<PagedResult<AnnouncementResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _announcementRepository.GetPaginatedAsync(request);

            return new PagedResult<AnnouncementResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<List<AnnouncementLookupResponse>> GetActiveBySiteIdAsync(long siteId)
        {
            var entities = await _announcementRepository.GetActiveBySiteIdAsync(siteId);
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }
    }
}