using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetEligible
{
    public class ElectionGetEligibleQuery : IRequest<List<ElectionEligibleResponse>>
    {
        public long EmployeeId { get; set; }

        public ElectionGetEligibleQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
