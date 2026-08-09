using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupMemberGetAll
{
    public class GroupMemberGetAllQuery : IRequest<List<GroupMemberResponse>>
    {
        public long GroupId { get; set; }

        public GroupMemberGetAllQuery(long groupId)
        {
            GroupId = groupId;
        }
    }
}
