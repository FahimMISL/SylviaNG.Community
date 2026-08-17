namespace SylviaNG.Community.Application.Features.Roles.Models
{
    public class RoleResponse
    {
        public long RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
