using MediatR;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupLeave
{
    public class GroupLeaveCommand : IRequest
    {
        public long GroupId { get; set; }
        public long CallerEmployeeId { get; set; }

        public GroupLeaveCommand(long groupId, long callerEmployeeId)
        {
            GroupId = groupId;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
