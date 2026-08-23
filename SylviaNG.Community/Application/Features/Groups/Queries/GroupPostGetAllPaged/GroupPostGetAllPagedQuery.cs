using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupPostGetAllPaged
{
    public class GroupPostGetAllPagedQuery : IRequest<PagedResult<PostResponse>>
    {
        public long GroupId { get; set; }
        public PostFilterRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public GroupPostGetAllPagedQuery(long groupId, PostFilterRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            GroupId = groupId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
