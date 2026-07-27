namespace SylviaNG.Community.Application.Features.RecognitionComments.Models
{
    public class RecognitionCommentAddRequest
    {
        public long EmployeeId { get; set; }
        public long? ParentCommentId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
