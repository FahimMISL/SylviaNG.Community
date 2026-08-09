using MediatR;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupJoinRequestCreate
{
    public class GroupJoinRequestCreateCommand : IRequest<long>
    {
        public long GroupId { get; set; }
        public long CallerEmployeeId { get; set; }

        public GroupJoinRequestCreateCommand(long groupId, long callerEmployeeId)
        {
            GroupId = groupId;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
