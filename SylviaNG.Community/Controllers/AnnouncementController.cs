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
    [Route("community/announcement")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnnouncementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all announcements.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<AnnouncementResponse>>> GetAll()
        {
            var result = await _mediator.Send(new AnnouncementGetAllQuery());
            return Ok(result);
        }

        /// <summary>
        /// Get an announcement by ID.
        /// </summary>
        [HttpGet("{announcementId}")]
        public async Task<ActionResult<AnnouncementResponse>> GetById(long announcementId)
        {
            var result = await _mediator.Send(new AnnouncementGetByIdQuery(announcementId));
            return Ok(result);
        }

        /// <summary>
        /// Get paginated announcements with search and sort.
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<AnnouncementResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new AnnouncementGetAllPagedQuery(request));
            return Ok(result);
        }

        /// <summary>
        /// Create a new announcement.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] AnnouncementCreateRequest request)
        {
            var id = await _mediator.Send(new AnnouncementCreateCommand(request));
            return Ok(id);
        }

        /// <summary>
        /// Update an existing announcement.
        /// </summary>
        [HttpPut("{announcementId}")]
        public async Task<ActionResult> Update(long announcementId, [FromBody] AnnouncementUpdateRequest request)
        {
            await _mediator.Send(new AnnouncementUpdateCommand(announcementId, request));
            return Ok();
        }

        /// <summary>
        /// Delete an announcement.
        /// </summary>
        [HttpDelete("{announcementId}")]
        public async Task<ActionResult> Delete(long announcementId)
        {
            await _mediator.Send(new AnnouncementDeleteCommand(announcementId));
            return Ok();
        }
    }
}