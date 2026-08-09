namespace SylviaNG.Community.Application.Features.Elections.Models
{
    public class ElectionCandidateNominateRequest
    {
        /// <summary>Set when nominating an individual Employee candidate.</summary>
        public long? EmployeeId { get; set; }

        /// <summary>Set when nominating a Team candidate. Exactly one of EmployeeId/TeamId must be set.</summary>
        public long? TeamId { get; set; }

        public string CandidateType { get; set; } = string.Empty;
        public string? Manifesto { get; set; }
    }
}
