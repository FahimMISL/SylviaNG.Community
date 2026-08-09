using MediatR;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupJoinRequestReject
{
    public class GroupJoinRequestRejectCommand : IRequest
    {
        public long GroupJoinRequestId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public GroupJoinRequestRejectCommand(long groupJoinRequestId, long callerEmployeeId, bool isHrOrAdmin)
        {
            GroupJoinRequestId = groupJoinRequestId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
