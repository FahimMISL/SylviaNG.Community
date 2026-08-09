namespace SylviaNG.Community.Application.Features.Branches.Models
{
    public class BranchResponse
    {
        public long BranchId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public long? CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
