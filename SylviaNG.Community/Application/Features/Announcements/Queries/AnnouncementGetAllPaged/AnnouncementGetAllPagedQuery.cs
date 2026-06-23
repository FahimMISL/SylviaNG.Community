using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetAllPaged
{
    public class AnnouncementGetAllPagedQuery : IRequest<PagedResult<AnnouncementResponse>>
    {
        public PagedRequest Request { get; set; }

        public AnnouncementGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
