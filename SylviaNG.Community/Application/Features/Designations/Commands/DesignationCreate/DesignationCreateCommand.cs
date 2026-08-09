using MediatR;
using SylviaNG.Community.Application.Features.Designations.Models;

namespace SylviaNG.Community.Application.Features.Designations.Commands.DesignationCreate
{
    public class DesignationCreateCommand : IRequest<long>
    {
        public DesignationCreateRequest Request { get; set; }

        public DesignationCreateCommand(DesignationCreateRequest request)
        {
            Request = request;
        }
    }
}
