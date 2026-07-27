using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ConversationGetAllPaged
{
    public class ConversationGetAllPagedQuery : IRequest<PagedResult<ConversationResponse>>
    {
        public long EmployeeId { get; set; }
        public PagedRequest Request { get; set; }

        public ConversationGetAllPagedQuery(long employeeId, PagedRequest request)
        {
            EmployeeId = employeeId;
            Request = request;
        }
    }
}
