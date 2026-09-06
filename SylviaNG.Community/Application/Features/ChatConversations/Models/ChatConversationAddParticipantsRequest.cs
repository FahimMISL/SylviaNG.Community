namespace SylviaNG.Community.Application.Features.ChatConversations.Models
{
    public class ChatConversationAddParticipantsRequest
    {
        public List<long> EmployeeIds { get; set; } = new();
    }
}
