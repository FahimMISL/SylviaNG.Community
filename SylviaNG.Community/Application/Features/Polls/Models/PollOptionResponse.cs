namespace SylviaNG.Community.Application.Features.Polls.Models
{
    public class PollOptionResponse
    {
        public long PollOptionId { get; set; }
        public long PollId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int VoteCount { get; set; }
    }
}
