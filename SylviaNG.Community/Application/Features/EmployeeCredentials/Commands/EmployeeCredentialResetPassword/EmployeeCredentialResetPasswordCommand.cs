using MediatR;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;

namespace SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialResetPassword
{
    public class EmployeeCredentialResetPasswordCommand : IRequest
    {
        public long EmployeeId { get; set; }
        public EmployeeCredentialResetPasswordRequest Request { get; set; }

        public EmployeeCredentialResetPasswordCommand(long employeeId, EmployeeCredentialResetPasswordRequest request)
        {
            EmployeeId = employeeId;
            Request = request;
        }
    }
}
