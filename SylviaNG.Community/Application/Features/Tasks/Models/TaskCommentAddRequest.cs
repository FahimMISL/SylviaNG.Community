namespace SylviaNG.Community.Application.Features.Tasks.Models
{
    public class TaskCommentAddRequest
    {
        public long EmployeeId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
