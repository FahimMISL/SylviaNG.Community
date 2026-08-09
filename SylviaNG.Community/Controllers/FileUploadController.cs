using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.FileStorages.Commands.FileStorageCreate;
using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    /// <summary>
    /// Accepts real file bytes (multipart/form-data) for Community attachments (currently
    /// post attachments - Phase 5 / US-3.4) and writes them to local disk under the configured
    /// FileStorage:LocalRootPath. This is the missing byte-storage piece that FileStorageController
    /// intentionally does not handle (it only persists metadata for uploads that already happened
    /// elsewhere). After a successful write, this controller creates that metadata record via the
    /// existing FileStorageCreateCommand/Handler (through IMediator, same as every other controller
    /// in this codebase) so the two layers stay consistent.
    ///
    /// Served-file URL shape: once Program.cs wires app.UseStaticFiles with RequestPath "/uploads"
    /// pointed at this same LocalRootPath, a file with StoragePath "uploads/{module}/{yyyy-MM}/{guid}.ext"
    /// is reachable at "{Base_URL}/uploads/{module}/{yyyy-MM}/{guid}.ext" (StoragePath already begins
    /// with the "uploads" segment, so the frontend just appends it to Base_URL, not BASE_URL_Community).
    /// </summary>
    [ApiController]
    [Route("community/file-upload")]
    public class FileUploadController : ControllerBase
    {
        private const long MaxFileSizeBytes = 25 * 1024 * 1024; // 25 MB

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".mov", ".webm"
        };

        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;

        public FileUploadController(
            IMediator mediator,
            IWebHostEnvironment environment,
            IConfiguration configuration,
            ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _environment = environment;
            _configuration = configuration;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<ActionResult<FileUploadResponse>> Upload([FromForm] IFormFile file, [FromForm] string module, [FromForm] long? entityId)
        {
            if (file is null || file.Length <= 0)
            {
                return BadRequest("No file was provided.");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                return BadRequest($"File exceeds the maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB.");
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                return BadRequest($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}.");
            }

            if (string.IsNullOrWhiteSpace(module))
            {
                return BadRequest("Module is required.");
            }

            var safeFileName = $"{Guid.NewGuid()}{extension}";

            // Subpath under both the physical LocalRootPath directory and the "/uploads" static
            // file RequestPath - the two are configured (in Program.cs) to point at the same
            // physical folder, so this one subpath serves double duty.
            var subPath = $"{module}/{DateTime.UtcNow:yyyy-MM}/{safeFileName}";

            // StoragePath persisted on the FileStorage record and returned to the client: prefixed
            // with "uploads" so "{Base_URL}/{StoragePath}" resolves once UseStaticFiles is wired
            // with RequestPath "/uploads" pointed at LocalRootPath (see class remarks above).
            var relativePath = $"uploads/{subPath}";

            var configuredRoot = _configuration["FileStorage:LocalRootPath"];
            var rootPath = string.IsNullOrWhiteSpace(configuredRoot) ? "wwwroot/uploads" : configuredRoot;

            // Path.Combine short-circuits to rootPath if it's rooted/absolute, so this works whether
            // FileStorage:LocalRootPath is configured as a relative ("wwwroot/uploads") or absolute path.
            var absolutePath = Path.Combine(_environment.ContentRootPath, rootPath, subPath);

            var absoluteDirectory = Path.GetDirectoryName(absolutePath);
            if (!string.IsNullOrEmpty(absoluteDirectory))
            {
                Directory.CreateDirectory(absoluteDirectory);
            }

            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var uploadedBy = _currentUserService.EmployeeId ?? 0;

            var fileId = await _mediator.Send(new FileStorageCreateCommand(new FileStorageCreateRequest
            {
                Module = module,
                EntityId = entityId,
                FileName = safeFileName,
                OriginalFileName = file.FileName,
                FileExtension = extension,
                MimeType = file.ContentType,
                FileSize = file.Length,
                StoragePath = relativePath,
                UploadedBy = uploadedBy
            }));

            return Ok(new FileUploadResponse
            {
                FileId = fileId,
                StoragePath = relativePath,
                OriginalFileName = file.FileName
            });
        }
    }
}
