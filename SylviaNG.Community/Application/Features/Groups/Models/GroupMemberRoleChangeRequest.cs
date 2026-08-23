using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Groups.Models
{
    /// <summary>
    /// Promoting to GroupAdmin can be done by the Creator or another GroupAdmin; setting
    /// NewRole to Creator is an ownership transfer and is restricted to the current Creator
    /// only - both rules are enforced in GroupService, not here.
    /// </summary>
    public class GroupMemberRoleChangeRequest
    {
        public long EmployeeId { get; set; }
        public GroupMemberRoleEnum NewRole { get; set; }
    }
}
