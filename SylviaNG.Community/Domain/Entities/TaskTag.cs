using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

public class TaskTag : Audit
{
    public long TagId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
