using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.FavoriteAdd
{
    public class FavoriteAddCommand : IRequest<long>
    {
        public long EmployeeId { get; set; }
        public FavoriteAddRequest Request { get; set; }

        public FavoriteAddCommand(long employeeId, FavoriteAddRequest request)
        {
            EmployeeId = employeeId;
            Request = request;
        }
    }
}
