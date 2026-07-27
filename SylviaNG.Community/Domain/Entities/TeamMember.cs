using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

public class TeamMember : Audit
{
    public long TeamMemberId { get; set; }
    public long TeamId { get; set; }
    public long EmployeeId { get; set; }
    public DateTime JoinedDate { get; set; }
    public bool IsActive { get; set; } = true;
}
