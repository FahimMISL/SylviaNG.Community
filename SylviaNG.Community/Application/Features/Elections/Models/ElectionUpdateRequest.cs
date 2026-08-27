namespace SylviaNG.Community.Application.Features.Elections.Models
{
    public class ElectionUpdateRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ElectionType { get; set; }
        public string? CandidateType { get; set; }
        public string? AudienceScope { get; set; }
        public bool? IsAnonymous { get; set; }
        public bool? AllowMultipleChoice { get; set; }
        public int? MinSelection { get; set; }
        public int? MaxSelection { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
