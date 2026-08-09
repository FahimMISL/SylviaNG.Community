using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Marketplace.Commands.FavoriteAdd;
using SylviaNG.Community.Application.Features.Marketplace.Commands.FavoriteRemove;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Marketplace.Queries.FavoriteGetAll;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/favorite")]
    public class FavoriteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public FavoriteController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FavoriteResponse>>> GetAll()
        {
            var employeeId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new FavoriteGetAllQuery(employeeId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Add([FromBody] FavoriteAddRequest request)
        {
            var employeeId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new FavoriteAddCommand(employeeId, request));
            return Ok(id);
        }

        [HttpDelete("{listingId}")]
        public async Task<ActionResult> Remove(long listingId)
        {
            var employeeId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new FavoriteRemoveCommand(employeeId, listingId));
            return Ok();
        }
    }
}
