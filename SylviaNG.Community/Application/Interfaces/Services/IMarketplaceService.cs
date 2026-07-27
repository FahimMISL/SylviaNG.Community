using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IMarketplaceService
    {
        // Listing
        Task<long> CreateListingAsync(long sellerId, ListingCreateRequest request);
        Task UpdateListingAsync(long listingId, long callerEmployeeId, bool isHrOrAdmin, ListingUpdateRequest request);
        Task ApproveListingAsync(long listingId, long approverId);
        Task RejectListingAsync(long listingId, long approverId, ListingRejectRequest request);
        Task<ListingResponse> GetListingByIdAsync(long listingId);
        Task<PagedResult<ListingResponse>> GetListingsPagedAsync(ListingFilterRequest request);

        // Listing images
        Task<long> AddImageAsync(long listingId, ListingImageAddRequest request);
        Task RemoveImageAsync(long listingId, long imageId);
        Task<List<ListingImageResponse>> GetImagesAsync(long listingId);

        // Favorites
        Task<long> AddFavoriteAsync(long employeeId, FavoriteAddRequest request);
        Task RemoveFavoriteAsync(long employeeId, long listingId);
        Task<List<FavoriteResponse>> GetFavoritesAsync(long employeeId);

        // Conversations
        Task<long> StartConversationAsync(long initiatorEmployeeId, ConversationStartRequest request);
        Task<ConversationResponse> GetConversationByIdAsync(long conversationId);
        Task<PagedResult<ConversationResponse>> GetConversationsPagedAsync(long employeeId, PagedRequest request);

        // Messages
        Task<long> SendMessageAsync(long conversationId, long senderId, MessageSendRequest request);
        Task<List<MessageResponse>> GetMessagesAsync(long conversationId);

        // Reports
        Task<long> CreateReportAsync(long reportedBy, MarketplaceReportCreateRequest request);
        Task<PagedResult<MarketplaceReportResponse>> GetReportsPagedAsync(PagedRequest request);
        Task ResolveReportAsync(long reportId, long reviewerId, MarketplaceReportResolveRequest request);
    }
}
