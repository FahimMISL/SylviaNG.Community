namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyOptionResultResponse
    {
        public long OptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
