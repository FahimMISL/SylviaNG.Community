using MediatR;
using SylviaNG.Community.Application.Features.Designations.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Designations.Queries.DesignationGetAllPaged
{
    public class DesignationGetAllPagedQuery : IRequest<PagedResult<DesignationResponse>>
    {
        public PagedRequest Request { get; set; }

        public DesignationGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
