using SylviaNG.Community.Application.Features.RecognitionComments.Models;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IRecognitionService
    {
        Task<long> CreateAsync(RecognitionCreateRequest request, long callerEmployeeId, bool isHrOrAdmin);
        Task<RecognitionResponse> GetByIdAsync(long recognitionId);
        Task<PagedResult<RecognitionResponse>> GetPaginatedAsync(PagedRequest request, long? senderId = null, long? recipientId = null, long? viewerEmployeeId = null, bool viewerIsHrAdmin = false);
        Task<long> AddReactionAsync(long recognitionId, RecognitionReactionAddRequest request, long callerEmployeeId);
        Task RemoveReactionAsync(long recognitionId, long employeeId, long callerEmployeeId, bool isHrOrAdmin);
        Task<List<RecognitionReactionResponse>> GetReactionsAsync(long recognitionId);
        Task<long> AddCommentAsync(long recognitionId, RecognitionCommentAddRequest request, long callerEmployeeId);
        Task<List<RecognitionCommentResponse>> GetCommentsAsync(long recognitionId);
    }
}
