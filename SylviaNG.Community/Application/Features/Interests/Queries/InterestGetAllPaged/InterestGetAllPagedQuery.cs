using MediatR;
using SylviaNG.Community.Application.Features.Interests.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Interests.Queries.InterestGetAllPaged
{
    public class InterestGetAllPagedQuery : IRequest<PagedResult<InterestResponse>>
    {
        public PagedRequest Request { get; set; }

        public InterestGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
