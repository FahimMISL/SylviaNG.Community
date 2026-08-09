using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Groups.Models
{
    public class GroupMemberResponse
    {
        public long GroupMemberId { get; set; }
        public long GroupId { get; set; }
        public long EmployeeId { get; set; }
        public GroupMemberRoleEnum Role { get; set; }
        public DateTime JoinedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
