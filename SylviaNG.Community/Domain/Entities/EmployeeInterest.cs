using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Junction entity tagging an Employee with an Interest from the catalog.
/// </summary>
public class EmployeeInterest : Audit
{
    public long EmployeeInterestId { get; set; }
    public long EmployeeId { get; set; }
    public long InterestId { get; set; }
}
