using MediatR;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupJoin
{
    public class GroupJoinCommand : IRequest
    {
        public long GroupId { get; set; }
        public long CallerEmployeeId { get; set; }

        public GroupJoinCommand(long groupId, long callerEmployeeId)
        {
            GroupId = groupId;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
