namespace SylviaNG.Community.Application.Features.Elections.Models
{
    /// <summary>
    /// An Open election the calling employee is eligible to vote in (US-9.8) - extends
    /// ElectionResponse's fields with whether they've already voted; the client derives
    /// "time remaining" from EndDate.
    /// </summary>
    public class ElectionEligibleResponse
    {
        public long ElectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ElectionType { get; set; } = string.Empty;
        public string CandidateType { get; set; } = string.Empty;
        public bool AllowMultipleChoice { get; set; }
        public int MinSelection { get; set; }
        public int MaxSelection { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool HasVoted { get; set; }
    }
}
