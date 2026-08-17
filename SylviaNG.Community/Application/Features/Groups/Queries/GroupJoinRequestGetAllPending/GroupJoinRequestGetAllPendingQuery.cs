using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupJoinRequestGetAllPending
{
    public class GroupJoinRequestGetAllPendingQuery : IRequest<List<GroupJoinRequestResponse>>
    {
        public long GroupId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public GroupJoinRequestGetAllPendingQuery(long groupId, long callerEmployeeId, bool isHrOrAdmin)
        {
            GroupId = groupId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
