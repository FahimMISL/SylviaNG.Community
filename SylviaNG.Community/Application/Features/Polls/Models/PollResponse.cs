namespace SylviaNG.Community.Application.Features.Polls.Models
{
    public class PollResponse
    {
        public long PollId { get; set; }
        public long PostId { get; set; }
        public bool AllowVoteChange { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<PollOptionResponse> Options { get; set; } = new();
    }
}
