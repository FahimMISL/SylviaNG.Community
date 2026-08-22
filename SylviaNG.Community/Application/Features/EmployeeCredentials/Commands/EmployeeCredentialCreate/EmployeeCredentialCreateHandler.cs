using MediatR;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialCreate
{
    public class EmployeeCredentialCreateHandler : IRequestHandler<EmployeeCredentialCreateCommand, EmployeeCredentialResponse>
    {
        private readonly IEmployeeCredentialService _employeeCredentialService;

        public EmployeeCredentialCreateHandler(IEmployeeCredentialService employeeCredentialService)
        {
            _employeeCredentialService = employeeCredentialService;
        }

        public async Task<EmployeeCredentialResponse> Handle(EmployeeCredentialCreateCommand command, CancellationToken cancellationToken)
        {
            return await _employeeCredentialService.CreateAsync(command.Request);
        }
    }
}
