using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// An interest-based group employees can create, join, and post within. Current
/// ownership (the "Creator") is derived from the GroupMember row with Role = Creator
/// rather than a field here, since ownership is transferable (see GroupMemberRoleEnum).
/// </summary>
public class Group : Audit
{
    public long GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GroupVisibilityEnum Visibility { get; set; } = GroupVisibilityEnum.Public;
}
