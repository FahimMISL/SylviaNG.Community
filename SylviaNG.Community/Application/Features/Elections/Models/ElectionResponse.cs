using System.Text.Json.Serialization;
using SylviaNG.Community.SharedKernel.Utils;

namespace SylviaNG.Community.Application.Features.Elections.Models
{
    public class ElectionResponse
    {
        public long ElectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ElectionType { get; set; } = string.Empty;
        public string CandidateType { get; set; } = string.Empty;
        public string AudienceScope { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
        public bool AllowMultipleChoice { get; set; }
        public int MinSelection { get; set; }
        public int MaxSelection { get; set; }

        // Clients compare these against their own clock (Vote page's open/closed check) - they
        // need a true, unambiguous UTC instant, not the app-wide LocalDateTimeJsonConverter's
        // marker-less business-local string.
        [JsonConverter(typeof(UtcDateTimeJsonConverter))]
        public DateTime StartDate { get; set; }

        [JsonConverter(typeof(NullableUtcDateTimeJsonConverter))]
        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = string.Empty;

        [JsonConverter(typeof(NullableUtcDateTimeJsonConverter))]
        public DateTime? PublishedAt { get; set; }

        public long? CreatedBy { get; set; }
    }
}
