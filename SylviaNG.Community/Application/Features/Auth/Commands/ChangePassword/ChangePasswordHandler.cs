using MediatR;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordHandler(IAuthService authService, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }

        public async Task Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var username = _currentUserService.Username
                ?? throw new ForbiddenException("Password changes are only available for locally-authenticated accounts.");

            await _authService.ChangePasswordAsync(username, command.Request);
        }
    }
}
