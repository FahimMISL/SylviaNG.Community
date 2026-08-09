using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Groups.Models
{
    public class GroupJoinRequestResponse
    {
        public long GroupJoinRequestId { get; set; }
        public long GroupId { get; set; }
        public long EmployeeId { get; set; }
        public GroupJoinRequestStatusEnum Status { get; set; }
        public long? ResolvedByEmployeeId { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
