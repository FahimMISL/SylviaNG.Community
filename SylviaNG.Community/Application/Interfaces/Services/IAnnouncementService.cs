using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IAnnouncementService
    {
        Task<long> CreateAsync(AnnouncementCreateRequest request);
        Task UpdateAsync(long AnnouncementId, AnnouncementUpdateRequest request);
        Task DeleteAsync(long AnnouncementId);
        Task<AnnouncementResponse> GetByIdAsync(long AnnouncementId);
        Task<List<AnnouncementResponse>> GetAllAsync();
        Task<PagedResult<AnnouncementResponse>> GetPaginatedAsync(PagedRequest request);
        Task<List<AnnouncementLookupResponse>> GetActiveBySiteIdAsync(long siteId);
    }
}
