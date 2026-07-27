using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Auth.Commands.Login;
using SylviaNG.Community.Application.Features.Auth.Models;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Local login for the admin UI (see JwtTokenGenerator) - issues a JWT accepted by
        /// the "Local" JwtBearer scheme registered in AuthenticationExtensions.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _mediator.Send(new LoginCommand(request));
            return Ok(result);
        }
    }
}
