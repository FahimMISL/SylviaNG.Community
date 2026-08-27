using SylviaNG.Community.Application.Features.Recognitions.Models;

namespace SylviaNG.Community.Application.Features.Dashboard.Models
{
    /// <summary>US-8.3: the HR/Admin company-wide operational summary.</summary>
    public class AdminDashboardSummaryResponse
    {
        public int ActiveSurveyCount { get; set; }

        /// <summary>
        /// Average of SurveyResultsResponse.ParticipationRate across Published surveys - null when
        /// no Published survey has a computable rate yet (mirrors SurveyResultsResponse's own
        /// EntireCompany-only limitation; render "N/A" on the frontend, same as that screen does).
        /// </summary>
        public decimal? AverageParticipationRate { get; set; }

        public List<RecognitionResponse> RecentRecognitions { get; set; } = new();
        public int PendingListingCount { get; set; }
    }
}
