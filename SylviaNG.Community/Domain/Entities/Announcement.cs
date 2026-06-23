using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Represents a job posting/requisition in the community system.
/// </summary>
public class Announcement : Audit
{
    public long AnnouncementId { get; set; }
    public long SiteId { get; set; }
    public long? DepartmentId { get; set; }
    public long? DesignationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public int NumberOfPositions { get; set; } = 1;
    public EmploymentTypeEnum EmploymentType { get; set; } = EmploymentTypeEnum.FullTime;
    public new JobStatusEnum Status { get; set; } = JobStatusEnum.Draft;
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public DateTime? PostingDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
