using MediatR;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;

namespace SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialCreate
{
    public class EmployeeCredentialCreateCommand : IRequest<EmployeeCredentialResponse>
    {
        public EmployeeCredentialCreateRequest Request { get; set; }

        public EmployeeCredentialCreateCommand(EmployeeCredentialCreateRequest request)
        {
            Request = request;
        }
    }
}
