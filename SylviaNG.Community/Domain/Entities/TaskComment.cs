using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

public class TaskComment : Audit
{
    public long CommentId { get; set; }
    public long TaskId { get; set; }
    public long EmployeeId { get; set; }
    public string Comment { get; set; } = string.Empty;
}
