namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskCommentResponse
    {
        public long CommentId { get; set; }
        public long TaskId { get; set; }
        public long EmployeeId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }
}
