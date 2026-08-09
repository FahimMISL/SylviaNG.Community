using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Junction entity recording a Badge awarded to an Employee.
/// </summary>
public class EmployeeBadge : Audit
{
    public long EmployeeBadgeId { get; set; }
    public long EmployeeId { get; set; }
    public long BadgeId { get; set; }
    public DateTime AwardedDate { get; set; }
}
