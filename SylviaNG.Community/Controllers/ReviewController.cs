using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ReviewCreate;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ReviewImageAdd;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Marketplace.Queries.ReviewGetAllForListing;
using SylviaNG.Community.Application.Features.Marketplace.Queries.ReviewImageGetAll;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/review")]
    public class ReviewController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ReviewController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("listing/{listingId}")]
        public async Task<ActionResult<List<ReviewResponse>>> GetForListing(long listingId)
        {
            var result = await _mediator.Send(new ReviewGetAllForListingQuery(listingId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ReviewCreateRequest request)
        {
            var reviewerId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new ReviewCreateCommand(reviewerId, request));
            return Ok(id);
        }

        [HttpGet("{reviewId}/images")]
        public async Task<ActionResult<List<ReviewImageResponse>>> GetImages(long reviewId)
        {
            var result = await _mediator.Send(new ReviewImageGetAllQuery(reviewId));
            return Ok(result);
        }

        [HttpPost("{reviewId}/images")]
        public async Task<ActionResult<long>> AddImage(long reviewId, [FromBody] ReviewImageAddRequest request)
        {
            var id = await _mediator.Send(new ReviewImageAddCommand(reviewId, request));
            return Ok(id);
        }
    }
}
