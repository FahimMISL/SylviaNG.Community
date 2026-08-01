namespace SylviaNG.Community.Application.Features.FileStorages.Models
{
    /// <summary>
    /// Returned by FileUploadController.Upload after bytes have been written to disk and a
    /// FileStorage metadata record has been created. StoragePath is the relative path a client
    /// can append to the static-files base URL to render/download the file; FileId lets the
    /// caller reference this upload later (e.g. from a PostAttachmentAddRequest-adjacent flow).
    /// </summary>
    public class FileUploadResponse
    {
        public long FileId { get; set; }
        public string StoragePath { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
    }
}
