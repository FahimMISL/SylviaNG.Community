using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Marketplace.Commands.PurchaseCreate;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Marketplace.Queries.PurchaseGetAllForEmployee;
using SylviaNG.Community.Application.Features.Marketplace.Queries.PurchaseHasPurchased;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/purchase")]
    public class PurchaseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public PurchaseController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] PurchaseCreateRequest request)
        {
            var buyerId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new PurchaseCreateCommand(buyerId, request));
            return Ok(id);
        }

        [HttpGet("mine")]
        public async Task<ActionResult<List<PurchaseResponse>>> GetMine()
        {
            var employeeId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new PurchaseGetAllForEmployeeQuery(employeeId));
            return Ok(result);
        }

        [HttpGet("has-purchased/{listingId}")]
        public async Task<ActionResult<bool>> HasPurchased(long listingId)
        {
            var employeeId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new PurchaseHasPurchasedQuery(employeeId, listingId));
            return Ok(result);
        }
    }
}
