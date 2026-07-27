using MediatR;
using SylviaNG.Community.Application.Features.Interests.Models;

namespace SylviaNG.Community.Application.Features.Interests.Commands.InterestCreate
{
    public class InterestCreateCommand : IRequest<long>
    {
        public InterestCreateRequest Request { get; set; }

        public InterestCreateCommand(InterestCreateRequest request)
        {
            Request = request;
        }
    }
}
