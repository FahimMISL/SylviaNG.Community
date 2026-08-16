using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingApprove;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingCreate;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingDelete;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingImageAdd;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingImageRemove;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingReject;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingUpdate;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Marketplace.Queries.ListingGetAllPaged;
using SylviaNG.Community.Application.Features.Marketplace.Queries.ListingGetById;
using SylviaNG.Community.Application.Features.Marketplace.Queries.ListingImageGetAll;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/listing")]
    public class ListingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ListingController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ListingResponse>>> GetPaged([FromQuery] ListingFilterRequest request)
        {
            var result = await _mediator.Send(new ListingGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{listingId}")]
        public async Task<ActionResult<ListingResponse>> GetById(long listingId)
        {
            var result = await _mediator.Send(new ListingGetByIdQuery(listingId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ListingCreateRequest request)
        {
            var sellerId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new ListingCreateCommand(sellerId, _currentUserService.IsHrOrAdmin, request));
            return Ok(id);
        }

        [HttpPut("{listingId}")]
        public async Task<ActionResult> Update(long listingId, [FromBody] ListingUpdateRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ListingUpdateCommand(listingId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpDelete("{listingId}")]
        public async Task<ActionResult> Delete(long listingId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ListingDeleteCommand(listingId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{listingId}/approve")]
        public async Task<ActionResult> Approve(long listingId)
        {
            var approverId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ListingApproveCommand(listingId, approverId));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{listingId}/reject")]
        public async Task<ActionResult> Reject(long listingId, [FromBody] ListingRejectRequest request)
        {
            var approverId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ListingRejectCommand(listingId, approverId, request));
            return Ok();
        }

        [HttpGet("{listingId}/images")]
        public async Task<ActionResult<List<ListingImageResponse>>> GetImages(long listingId)
        {
            var result = await _mediator.Send(new ListingImageGetAllQuery(listingId));
            return Ok(result);
        }

        [HttpPost("{listingId}/images")]
        public async Task<ActionResult<long>> AddImage(long listingId, [FromBody] ListingImageAddRequest request)
        {
            var id = await _mediator.Send(new ListingImageAddCommand(listingId, request));
            return Ok(id);
        }

        [HttpDelete("{listingId}/images/{imageId}")]
        public async Task<ActionResult> RemoveImage(long listingId, long imageId)
        {
            await _mediator.Send(new ListingImageRemoveCommand(listingId, imageId));
            return Ok();
        }
    }
}
