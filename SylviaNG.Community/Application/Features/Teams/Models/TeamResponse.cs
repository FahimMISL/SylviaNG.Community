namespace SylviaNG.Community.Application.Features.Teams.Models
{
    public class TeamResponse
    {
        public long TeamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? SupervisorId { get; set; }
        public long? CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
