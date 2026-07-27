namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class ConversationResponse
    {
        public long ConversationId { get; set; }
        public long ListingId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ConversationParticipantResponse> Participants { get; set; } = new();
    }
}
