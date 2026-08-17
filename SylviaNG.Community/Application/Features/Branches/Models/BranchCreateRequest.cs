namespace SylviaNG.Community.Application.Features.Branches.Models
{
    public class BranchCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
