using MediatR;
using SylviaNG.Community.Application.Features.Auth.Models;

namespace SylviaNG.Community.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest
    {
        public ChangePasswordRequestDto Request { get; set; }

        public ChangePasswordCommand(ChangePasswordRequestDto request)
        {
            Request = request;
        }
    }
}
