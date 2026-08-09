using MediatR;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupDelete
{
    public class GroupDeleteCommand : IRequest
    {
        public long GroupId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public GroupDeleteCommand(long groupId, long callerEmployeeId, bool isHrOrAdmin)
        {
            GroupId = groupId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
