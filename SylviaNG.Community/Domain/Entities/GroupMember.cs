using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Removal/leave is a soft delete (IsActive = false) so join/approval history is retained,
/// matching TeamMember's convention. Exactly one active row per group should have
/// Role = Creator at any time; that invariant is enforced in GroupService, not here.
/// </summary>
public class GroupMember : Audit
{
    public long GroupMemberId { get; set; }
    public long GroupId { get; set; }
    public long EmployeeId { get; set; }
    public GroupMemberRoleEnum Role { get; set; } = GroupMemberRoleEnum.Member;
    public DateTime JoinedDate { get; set; }
    public bool IsActive { get; set; } = true;
}
