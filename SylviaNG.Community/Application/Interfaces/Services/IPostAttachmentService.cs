using SylviaNG.Community.Application.Features.PostAttachments.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IPostAttachmentService
    {
        Task<long> AddAsync(long postId, PostAttachmentAddRequest request);
        Task<List<PostAttachmentResponse>> GetByPostIdAsync(long postId);
        Task RemoveAsync(long postId, long attachmentId);
    }
}
