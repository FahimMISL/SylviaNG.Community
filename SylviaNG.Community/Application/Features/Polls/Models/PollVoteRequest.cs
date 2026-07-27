namespace SylviaNG.Community.Application.Features.Polls.Models
{
    public class PollVoteRequest
    {
        public long EmployeeId { get; set; }
        public long PollOptionId { get; set; }
    }
}
