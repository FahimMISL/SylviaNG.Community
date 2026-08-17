using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Kept as a status flip rather than deleted on reject, so a rejected request can be
/// told apart from a fresh one when the employee requests to join again later.
/// </summary>
public class GroupJoinRequest : Audit
{
    public long GroupJoinRequestId { get; set; }
    public long GroupId { get; set; }
    public long EmployeeId { get; set; }
    public new GroupJoinRequestStatusEnum Status { get; set; } = GroupJoinRequestStatusEnum.Pending;
    public long? ResolvedByEmployeeId { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
