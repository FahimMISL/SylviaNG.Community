using MediatR;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberRemove
{
    public class GroupMemberRemoveCommand : IRequest
    {
        public long GroupId { get; set; }
        public long EmployeeId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public GroupMemberRemoveCommand(long groupId, long employeeId, long callerEmployeeId, bool isHrOrAdmin)
        {
            GroupId = groupId;
            EmployeeId = employeeId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
