using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberRoleChange
{
    public class GroupMemberRoleChangeCommand : IRequest
    {
        public long GroupId { get; set; }
        public GroupMemberRoleChangeRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public GroupMemberRoleChangeCommand(long groupId, GroupMemberRoleChangeRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            GroupId = groupId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
