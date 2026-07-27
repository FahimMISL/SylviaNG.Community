namespace SylviaNG.Community.Application.Features.RecognitionComments.Models
{
    public class RecognitionCommentResponse
    {
        public long CommentId { get; set; }
        public long RecognitionId { get; set; }
        public long EmployeeId { get; set; }
        public long? ParentCommentId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }
}
