using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialResetPassword
{
    public class EmployeeCredentialResetPasswordHandler : IRequestHandler<EmployeeCredentialResetPasswordCommand>
    {
        private readonly IEmployeeCredentialService _employeeCredentialService;

        public EmployeeCredentialResetPasswordHandler(IEmployeeCredentialService employeeCredentialService)
        {
            _employeeCredentialService = employeeCredentialService;
        }

        public async Task Handle(EmployeeCredentialResetPasswordCommand command, CancellationToken cancellationToken)
        {
            await _employeeCredentialService.ResetPasswordAsync(command.EmployeeId, command.Request.TemporaryPassword);
        }
    }
}
