namespace SylviaNG.Community.Application.Features.Elections.Models
{
    public class ElectionAudienceTargetResponse
    {
        public long ElectionAudienceTargetId { get; set; }
        public long ElectionId { get; set; }
        public string TargetId { get; set; } = string.Empty;
    }
}
