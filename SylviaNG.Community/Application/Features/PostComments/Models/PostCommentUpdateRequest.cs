namespace SylviaNG.Community.Application.Features.PostComments.Models
{
    public class PostCommentUpdateRequest
    {
        public string Content { get; set; } = string.Empty;
        public List<long>? MentionedEmployeeIds { get; set; }
    }
}
