namespace SylviaNG.Community.Application.Features.PostComments.Models
{
    public class PostCommentResponse
    {
        public long CommentId { get; set; }
        public long PostId { get; set; }
        public long EmployeeId { get; set; }
        public long? ParentCommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }
}
