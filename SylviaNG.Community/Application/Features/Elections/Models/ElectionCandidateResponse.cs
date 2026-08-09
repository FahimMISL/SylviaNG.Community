namespace SylviaNG.Community.Application.Features.Elections.Models
{
    public class ElectionCandidateResponse
    {
        public long ElectionCandidateId { get; set; }
        public long ElectionId { get; set; }
        public long? EmployeeId { get; set; }
        public long? TeamId { get; set; }
        public string CandidateType { get; set; } = string.Empty;
        public string? Manifesto { get; set; }
        public bool IsApproved { get; set; }
        public DateTime NominatedAt { get; set; }
    }
}
