using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;

namespace SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetAll
{
    public class AnnouncementGetAllQuery : IRequest<List<AnnouncementResponse>>
    {
    }
}
