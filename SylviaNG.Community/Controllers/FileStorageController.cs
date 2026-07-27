using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.FileStorages.Commands.FileStorageCreate;
using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.Application.Features.FileStorages.Queries.FileStorageGetAllPaged;
using SylviaNG.Community.Application.Features.FileStorages.Queries.FileStorageGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    /// <summary>
    /// FileStorage tracks upload metadata only - actual byte storage/upload handling is out
    /// of scope. Records are created after an upload happens elsewhere.
    /// </summary>
    [ApiController]
    [Route("community/file-storage")]
    public class FileStorageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FileStorageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<FileStorageResponse>>> GetPaged(
            [FromQuery] PagedRequest request,
            [FromQuery] string? module,
            [FromQuery] long? entityId)
        {
            var result = await _mediator.Send(new FileStorageGetAllPagedQuery(request, module, entityId));
            return Ok(result);
        }

        [HttpGet("{fileId}")]
        public async Task<ActionResult<FileStorageResponse>> GetById(long fileId)
        {
            var result = await _mediator.Send(new FileStorageGetByIdQuery(fileId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] FileStorageCreateRequest request)
        {
            var id = await _mediator.Send(new FileStorageCreateCommand(request));
            return Ok(id);
        }
    }
}
