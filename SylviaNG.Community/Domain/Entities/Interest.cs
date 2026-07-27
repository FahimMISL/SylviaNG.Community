using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Catalog of interests that employees can be tagged with. Name is unique.
/// </summary>
public class Interest : Audit
{
    public long InterestId { get; set; }
    public string Name { get; set; } = string.Empty;
}
