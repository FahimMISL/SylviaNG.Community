namespace SylviaNG.Community.Application.Features.Branches.Models
{
    public class BranchUpdateRequest
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool? IsActive { get; set; }
    }
}
