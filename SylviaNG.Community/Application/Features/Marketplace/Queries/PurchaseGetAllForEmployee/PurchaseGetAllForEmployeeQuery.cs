using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.PurchaseGetAllForEmployee
{
    public class PurchaseGetAllForEmployeeQuery : IRequest<List<PurchaseResponse>>
    {
        public long EmployeeId { get; set; }

        public PurchaseGetAllForEmployeeQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
