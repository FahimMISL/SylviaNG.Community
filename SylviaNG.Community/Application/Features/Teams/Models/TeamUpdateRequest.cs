namespace SylviaNG.Community.Application.Features.Teams.Models
{
    public class TeamUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long? SupervisorId { get; set; }
        public bool? IsActive { get; set; }
    }
}
