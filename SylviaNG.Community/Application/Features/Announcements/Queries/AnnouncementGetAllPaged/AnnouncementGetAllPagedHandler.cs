using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetAllPaged
{
    public class AnnouncementGetAllPagedHandler : IRequestHandler<AnnouncementGetAllPagedQuery, PagedResult<AnnouncementResponse>>
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementGetAllPagedHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<PagedResult<AnnouncementResponse>> Handle(AnnouncementGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _announcementService.GetPaginatedAsync(query.Request);
        }
    }
}
