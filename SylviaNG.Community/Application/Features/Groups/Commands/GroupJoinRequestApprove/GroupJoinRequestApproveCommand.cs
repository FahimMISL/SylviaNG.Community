using MediatR;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupJoinRequestApprove
{
    public class GroupJoinRequestApproveCommand : IRequest
    {
        public long GroupJoinRequestId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public GroupJoinRequestApproveCommand(long groupJoinRequestId, long callerEmployeeId, bool isHrOrAdmin)
        {
            GroupJoinRequestId = groupJoinRequestId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
