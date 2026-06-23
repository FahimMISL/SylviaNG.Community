using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetAllPaged
{
    public class AnnouncementGetAllPagedHandler : IRequestHandler<AnnouncementGetAllPagedQuery, PagedResult<AnnouncementResponse>>
    {
        private readonly IAnnouncementService _AnnouncementService;

        public AnnouncementGetAllPagedHandler(IAnnouncementService AnnouncementService)
        {
            _AnnouncementService = AnnouncementService;
        }

        public async Task<PagedResult<AnnouncementResponse>> Handle(AnnouncementGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _AnnouncementService.GetPaginatedAsync(query.Request);
        }
    }
}
