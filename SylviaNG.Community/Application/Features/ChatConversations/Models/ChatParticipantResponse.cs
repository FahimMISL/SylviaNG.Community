namespace SylviaNG.Community.Application.Features.ChatConversations.Models
{
    public class ChatParticipantResponse
    {
        public long ChatParticipantId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? EmployeePhotoUrl { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LastReadAt { get; set; }
        public bool IsMuted { get; set; }
        public bool IsPinned { get; set; }
    }
}
