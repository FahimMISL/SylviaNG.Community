using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Marketplace.Commands.MarketplaceReportCreate;
using SylviaNG.Community.Application.Features.Marketplace.Commands.MarketplaceReportResolve;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Marketplace.Queries.MarketplaceReportGetAllPaged;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/marketplace-report")]
    public class MarketplaceReportController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public MarketplaceReportController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] MarketplaceReportCreateRequest request)
        {
            var reportedBy = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new MarketplaceReportCreateCommand(reportedBy, request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<MarketplaceReportResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new MarketplaceReportGetAllPagedQuery(request));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{reportId}/resolve")]
        public async Task<ActionResult> Resolve(long reportId, [FromBody] MarketplaceReportResolveRequest request)
        {
            var reviewerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new MarketplaceReportResolveCommand(reportId, reviewerId, request));
            return Ok();
        }
    }
}
