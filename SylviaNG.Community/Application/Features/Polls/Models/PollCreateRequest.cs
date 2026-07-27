namespace SylviaNG.Community.Application.Features.Polls.Models
{
    public class PollCreateRequest
    {
        public bool AllowVoteChange { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<string> Options { get; set; } = new();
    }
}
