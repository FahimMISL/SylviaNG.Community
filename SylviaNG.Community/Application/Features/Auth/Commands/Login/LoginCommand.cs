using MediatR;
using SylviaNG.Community.Application.Features.Auth.Models;

namespace SylviaNG.Community.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<LoginResponseDto>
    {
        public LoginRequestDto Request { get; set; }

        public LoginCommand(LoginRequestDto request)
        {
            Request = request;
        }
    }
}
