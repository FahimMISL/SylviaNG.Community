using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionAudienceTargetAdd;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateApprove;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateNominate;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionClose;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionCreate;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionDelete;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionPublish;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionUpdate;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionVoteCast;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Features.Elections.Queries.ElectionAudienceTargetGetAll;
using SylviaNG.Community.Application.Features.Elections.Queries.ElectionCandidateGetAll;
using SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetAllPaged;
using SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetById;
using SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetEligible;
using SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetResults;
using SylviaNG.Community.Application.Features.Elections.Queries.ElectionVoteGetAllPaged;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/election")]
    public class ElectionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ElectionController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ElectionResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ElectionGetAllPagedQuery(request));
            return Ok(result);
        }

        // Registered above the "{electionId}" route so "eligible" is never captured by the
        // long route-parameter binder.
        [HttpGet("eligible")]
        public async Task<ActionResult<List<ElectionEligibleResponse>>> GetEligible()
        {
            var employeeId = _currentUserService.EmployeeId
                ?? throw new UnauthorizedException("Only authenticated employees may browse eligible elections.");

            var result = await _mediator.Send(new ElectionGetEligibleQuery(employeeId));
            return Ok(result);
        }

        [HttpGet("{electionId}")]
        public async Task<ActionResult<ElectionResponse>> GetById(long electionId)
        {
            var result = await _mediator.Send(new ElectionGetByIdQuery(electionId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ElectionCreateRequest request)
        {
            var id = await _mediator.Send(new ElectionCreateCommand(request, _currentUserService.EmployeeId));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{electionId}")]
        public async Task<ActionResult> Update(long electionId, [FromBody] ElectionUpdateRequest request)
        {
            await _mediator.Send(new ElectionUpdateCommand(electionId, request));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{electionId}")]
        public async Task<ActionResult> Delete(long electionId)
        {
            await _mediator.Send(new ElectionDeleteCommand(electionId));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{electionId}/publish")]
        public async Task<ActionResult> Publish(long electionId)
        {
            await _mediator.Send(new ElectionPublishCommand(electionId));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{electionId}/close")]
        public async Task<ActionResult> Close(long electionId)
        {
            await _mediator.Send(new ElectionCloseCommand(electionId));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("{electionId}/results")]
        public async Task<ActionResult<ElectionResultsResponse>> GetResults(long electionId)
        {
            var result = await _mediator.Send(new ElectionGetResultsQuery(electionId));
            return Ok(result);
        }

        // Audience targeting - HRAdminOnly, since it controls who an election is visible/open to.
        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("{electionId}/audience")]
        public async Task<ActionResult<List<ElectionAudienceTargetResponse>>> GetAudience(long electionId)
        {
            var result = await _mediator.Send(new ElectionAudienceTargetGetAllQuery(electionId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost("{electionId}/audience")]
        public async Task<ActionResult<long>> AddAudience(long electionId, [FromBody] ElectionAudienceTargetAddRequest request)
        {
            var id = await _mediator.Send(new ElectionAudienceTargetAddCommand(electionId, request));
            return Ok(id);
        }

        // Candidates - listing/nominating stays open to any authenticated employee;
        // approval is HRAdminOnly.
        [HttpGet("{electionId}/candidates")]
        public async Task<ActionResult<List<ElectionCandidateResponse>>> GetCandidates(long electionId)
        {
            var result = await _mediator.Send(new ElectionCandidateGetAllQuery(electionId));
            return Ok(result);
        }

        [HttpPost("{electionId}/candidates")]
        public async Task<ActionResult<long>> Nominate(long electionId, [FromBody] ElectionCandidateNominateRequest request)
        {
            var id = await _mediator.Send(new ElectionCandidateNominateCommand(electionId, request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{electionId}/candidates/{candidateId}/approve")]
        public async Task<ActionResult> ApproveCandidate(long electionId, long candidateId)
        {
            await _mediator.Send(new ElectionCandidateApproveCommand(electionId, candidateId));
            return Ok();
        }

        // Votes - casting stays open to any authenticated employee (business rules enforced
        // in ElectionService.CastVoteAsync). Reading results is also left open rather than
        // HRAdminOnly: ElectionMapper.ToResponse already hides VoterId when the election is
        // anonymous, which was judged sufficient protection - see the tradeoff comment on
        // ElectionVoteResponse.VoterId.
        [HttpPost("{electionId}/votes")]
        public async Task<ActionResult<List<long>>> CastVote(long electionId, [FromBody] ElectionVoteCastRequest request)
        {
            var voterId = _currentUserService.EmployeeId
                ?? throw new UnauthorizedException("Only authenticated employees may cast a vote.");

            var ids = await _mediator.Send(new ElectionVoteCastCommand(electionId, request, voterId));
            return Ok(ids);
        }

        [HttpGet("{electionId}/votes")]
        public async Task<ActionResult<PagedResult<ElectionVoteResponse>>> GetVotes(long electionId, [FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ElectionVoteGetAllPagedQuery(electionId, request));
            return Ok(result);
        }
    }
}
