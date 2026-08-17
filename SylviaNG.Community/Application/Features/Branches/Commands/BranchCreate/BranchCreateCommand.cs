using MediatR;
using SylviaNG.Community.Application.Features.Branches.Models;

namespace SylviaNG.Community.Application.Features.Branches.Commands.BranchCreate
{
    public class BranchCreateCommand : IRequest<long>
    {
        public BranchCreateRequest Request { get; set; }

        public BranchCreateCommand(BranchCreateRequest request)
        {
            Request = request;
        }
    }
}
