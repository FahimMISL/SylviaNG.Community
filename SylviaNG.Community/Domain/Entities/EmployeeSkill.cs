using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Junction entity tagging an Employee with a Skill from the catalog.
/// </summary>
public class EmployeeSkill : Audit
{
    public long EmployeeSkillId { get; set; }
    public long EmployeeId { get; set; }
    public long SkillId { get; set; }
}
