using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;

namespace SylviaNG.Community.Application.Features.Groups.Commands.GroupUpdate
{
    public class GroupUpdateCommand : IRequest
    {
        public long GroupId { get; set; }
        public GroupUpdateRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public GroupUpdateCommand(long groupId, GroupUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            GroupId = groupId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
