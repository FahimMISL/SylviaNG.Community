using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberAdd
{
    public class GroupMemberAddCommand : IRequest<long>
    {
        public long GroupId { get; set; }
        public GroupMemberAddRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public GroupMemberAddCommand(long groupId, GroupMemberAddRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            GroupId = groupId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
