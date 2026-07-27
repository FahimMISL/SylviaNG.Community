using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.FavoriteGetAll
{
    public class FavoriteGetAllQuery : IRequest<List<FavoriteResponse>>
    {
        public long EmployeeId { get; set; }

        public FavoriteGetAllQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
