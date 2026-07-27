namespace SylviaNG.Community.Application.Features.Polls.Models
{
    public class PollVoteResponse
    {
        public long VoteId { get; set; }
        public long PollOptionId { get; set; }
        public long EmployeeId { get; set; }
        public DateTime VotedAt { get; set; }
    }
}
