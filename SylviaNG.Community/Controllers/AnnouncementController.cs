using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementCreate;
using SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementDelete;
using SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementUpdate;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetAll;
using SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetAllPaged;
using SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/job-posting")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnnouncementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all job postings.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<AnnouncementResponse>>> GetAll()
        {
            var result = await _mediator.Send(new AnnouncementGetAllQuery());
            return Ok(result);
        }

        /// <summary>
        /// Get a job posting by ID.
        /// </summary>
        [HttpGet("{AnnouncementId}")]
        public async Task<ActionResult<AnnouncementResponse>> GetById(long AnnouncementId)
        {
            var result = await _mediator.Send(new AnnouncementGetByIdQuery(AnnouncementId));
            return Ok(result);
        }

        /// <summary>
        /// Get paginated job postings with search and sort.
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<AnnouncementResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new AnnouncementGetAllPagedQuery(request));
            return Ok(result);
        }

        /// <summary>
        /// Create a new job posting.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] AnnouncementCreateRequest request)
        {
            var id = await _mediator.Send(new AnnouncementCreateCommand(request));
            return Ok(id);
        }

        /// <summary>
        /// Update an existing job posting.
        /// </summary>
        [HttpPut("{AnnouncementId}")]
        public async Task<ActionResult> Update(long AnnouncementId, [FromBody] AnnouncementUpdateRequest request)
        {
            await _mediator.Send(new AnnouncementUpdateCommand(AnnouncementId, request));
            return Ok();
        }

        /// <summary>
        /// Delete a job posting.
        /// </summary>
        [HttpDelete("{AnnouncementId}")]
        public async Task<ActionResult> Delete(long AnnouncementId)
        {
            await _mediator.Send(new AnnouncementDeleteCommand(AnnouncementId));
            return Ok();
        }
    }
}
