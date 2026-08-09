using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Catalog of skills that employees can be tagged with. Name is unique.
/// </summary>
public class Skill : Audit
{
    public long SkillId { get; set; }
    public string Name { get; set; } = string.Empty;
}
