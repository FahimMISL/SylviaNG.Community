using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupGetById
{
    public class GroupGetByIdQuery : IRequest<GroupResponse>
    {
        public long GroupId { get; set; }

        public GroupGetByIdQuery(long groupId)
        {
            GroupId = groupId;
        }
    }
}
