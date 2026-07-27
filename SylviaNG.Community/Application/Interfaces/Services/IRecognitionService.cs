using SylviaNG.Community.Application.Features.RecognitionComments.Models;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IRecognitionService
    {
        Task<long> CreateAsync(RecognitionCreateRequest request);
        Task<RecognitionResponse> GetByIdAsync(long recognitionId);
        Task<PagedResult<RecognitionResponse>> GetPaginatedAsync(PagedRequest request, long? senderId = null, long? recipientId = null);
        Task<long> AddReactionAsync(long recognitionId, RecognitionReactionAddRequest request);
        Task RemoveReactionAsync(long recognitionId, long employeeId);
        Task<List<RecognitionReactionResponse>> GetReactionsAsync(long recognitionId);
        Task<long> AddCommentAsync(long recognitionId, RecognitionCommentAddRequest request);
        Task<List<RecognitionCommentResponse>> GetCommentsAsync(long recognitionId);
    }
}
