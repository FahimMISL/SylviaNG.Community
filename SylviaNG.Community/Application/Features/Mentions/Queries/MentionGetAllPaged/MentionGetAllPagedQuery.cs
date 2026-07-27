using MediatR;
using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Mentions.Queries.MentionGetAllPaged
{
    public class MentionGetAllPagedQuery : IRequest<PagedResult<MentionResponse>>
    {
        public long MentionedEmployeeId { get; set; }
        public PagedRequest Request { get; set; }

        public MentionGetAllPagedQuery(long mentionedEmployeeId, PagedRequest request)
        {
            MentionedEmployeeId = mentionedEmployeeId;
            Request = request;
        }
    }
}
