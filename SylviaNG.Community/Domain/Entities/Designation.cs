using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

public class Designation : Audit
{
    public long DesignationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Grade { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
