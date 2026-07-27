namespace SylviaNG.Community.Application.Features.EmployeeBadges.Models
{
    public class EmployeeBadgeResponse
    {
        public long EmployeeBadgeId { get; set; }
        public long EmployeeId { get; set; }
        public long BadgeId { get; set; }
        public DateTime AwardedDate { get; set; }
    }
}
