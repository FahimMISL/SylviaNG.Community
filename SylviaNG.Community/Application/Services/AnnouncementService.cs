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
        private readonly IAnnouncementRepository _AnnouncementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AnnouncementService(
            IAnnouncementRepository AnnouncementRepository,
            IUnitOfWork _unitOfWork)
        {
            _AnnouncementRepository = AnnouncementRepository;
            this._unitOfWork = _unitOfWork;
        }

        public async Task<long> CreateAsync(AnnouncementCreateRequest request)
        {
            var exists = await _AnnouncementRepository.ExistsByTitleAndSiteIdAsync(request.Title, request.SiteId);
            if (exists)
                throw new DuplicateException("Announcement", "Title", request.Title);

            var entity = request.ToEntity();
            await _AnnouncementRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.AnnouncementId;
        }

        public async Task UpdateAsync(long AnnouncementId, AnnouncementUpdateRequest request)
        {
            var entity = await _AnnouncementRepository.GetByIdAsync(AnnouncementId)
                ?? throw new NotFoundException("Announcement", AnnouncementId);

            entity.ApplyUpdate(request);
            _AnnouncementRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long AnnouncementId)
        {
            var entity = await _AnnouncementRepository.GetByIdAsync(AnnouncementId)
                ?? throw new NotFoundException("Announcement", AnnouncementId);

            _AnnouncementRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AnnouncementResponse> GetByIdAsync(long AnnouncementId)
        {
            var entity = await _AnnouncementRepository.GetByIdWithIncludeAsync(
                j => j.AnnouncementId == AnnouncementId,
                j => j.Applications)
                ?? throw new NotFoundException("Announcement", AnnouncementId);

            return entity.ToResponse();
        }

        public async Task<List<AnnouncementResponse>> GetAllAsync()
        {
            var entities = await _AnnouncementRepository.GetAllWithIncludeAsync(j => j.Applications);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<PagedResult<AnnouncementResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _AnnouncementRepository.GetPaginatedAsync(request);

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
            var entities = await _AnnouncementRepository.GetActiveBySiteIdAsync(siteId);
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }
    }
}
