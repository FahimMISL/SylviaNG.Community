namespace SylviaNG.Community.Application.Features.PostComments.Models
{
    public class PostCommentAddRequest
    {
        public long EmployeeId { get; set; }
        public long? ParentCommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<long>? MentionedEmployeeIds { get; set; }
    }
}
