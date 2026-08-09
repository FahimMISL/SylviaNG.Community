namespace SylviaNG.Community.Application.Features.Teams.Models
{
    public class TeamMemberResponse
    {
        public long TeamMemberId { get; set; }
        public long TeamId { get; set; }
        public long EmployeeId { get; set; }
        public DateTime JoinedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
