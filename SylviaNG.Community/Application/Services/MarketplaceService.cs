using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IListingRepository _listingRepository;
        private readonly IListingImageRepository _listingImageRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IConversationParticipantRepository _conversationParticipantRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMarketplaceReportRepository _marketplaceReportRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewImageRepository _reviewImageRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public MarketplaceService(
            IListingRepository listingRepository,
            IListingImageRepository listingImageRepository,
            IFavoriteRepository favoriteRepository,
            IConversationRepository conversationRepository,
            IConversationParticipantRepository conversationParticipantRepository,
            IMessageRepository messageRepository,
            IMarketplaceReportRepository marketplaceReportRepository,
            IPurchaseRepository purchaseRepository,
            IReviewRepository reviewRepository,
            IReviewImageRepository reviewImageRepository,
            IEmployeeRepository employeeRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _listingRepository = listingRepository;
            _listingImageRepository = listingImageRepository;
            _favoriteRepository = favoriteRepository;
            _conversationRepository = conversationRepository;
            _conversationParticipantRepository = conversationParticipantRepository;
            _messageRepository = messageRepository;
            _marketplaceReportRepository = marketplaceReportRepository;
            _purchaseRepository = purchaseRepository;
            _reviewRepository = reviewRepository;
            _reviewImageRepository = reviewImageRepository;
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        private async Task<string> GetEmployeeNameAsync(long employeeId) =>
            (await _employeeRepository.GetByIdAsync(employeeId))?.EmployeeName ?? "Someone";

        // ---------------- Listing ----------------

        public async Task<long> CreateListingAsync(long sellerId, bool isHrOrAdmin, ListingCreateRequest request)
        {
            var entity = request.ToEntity(sellerId);

            if (isHrOrAdmin)
            {
                entity.Status = "Active";
                entity.ApprovalStatus = "Approved";
                entity.ApprovedBy = sellerId;
                entity.ApprovedAt = DateTime.UtcNow;
            }
            else if (request.SaveAsDraft)
            {
                entity.Status = "Draft";
                entity.ApprovalStatus = "Draft";
            }
            else
            {
                entity.Status = "Active";
                entity.ApprovalStatus = "Pending";
            }

            await _listingRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ListingId;
        }

        public async System.Threading.Tasks.Task UpdateListingAsync(long listingId, long callerEmployeeId, bool isHrOrAdmin, ListingUpdateRequest request)
        {
            var entity = await _listingRepository.GetByIdAsync(listingId)
                ?? throw new NotFoundException("Listing", listingId);

            if (!isHrOrAdmin && entity.SellerId != callerEmployeeId)
                throw new ForbiddenException("You can only edit your own listing.");

            entity.ApplyUpdate(request);
            ApplyStockRules(entity);

            if (request.Status == "Active" && entity.Quantity <= 0)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(entity.Status), "Cannot reactivate a listing with 0 quantity. Restock first.")
                });
            }

            ApplyApprovalWorkflowRules(entity, isHrOrAdmin, callerEmployeeId);

            _listingRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteListingAsync(long listingId, long callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _listingRepository.GetByIdAsync(listingId)
                ?? throw new NotFoundException("Listing", listingId);

            if (!isHrOrAdmin && entity.SellerId != callerEmployeeId)
                throw new ForbiddenException("You can only delete your own listing.");

            var conversations = await _conversationRepository.GetByListingIdAsync(listingId);
            foreach (var conversation in conversations.Where(c => c.Status != "Closed"))
            {
                conversation.Status = "Closed";
                _conversationRepository.Update(conversation);
            }

            entity.Status = "Removed";
            _listingRepository.Update(entity);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Re-review workflow (US-6.8/US-6.10): whenever a save resolves the listing's Status to
        /// "Active", a regular employee's edit always lands back in Pending review - whether that's
        /// a plain field edit on an already-approved listing or reactivating a Sold one - while an
        /// HR/Admin save is always immediately Approved. Status values other than "Active" (Sold,
        /// Draft) don't need re-vetting, so ApprovalStatus is left untouched for those.
        /// </summary>
        private static void ApplyApprovalWorkflowRules(Listing entity, bool isHrOrAdmin, long callerEmployeeId)
        {
            if (entity.Status != "Active")
                return;

            if (isHrOrAdmin)
            {
                entity.ApprovalStatus = "Approved";
                entity.ApprovedBy = callerEmployeeId;
                entity.ApprovedAt = DateTime.UtcNow;
                entity.RejectionReason = null;
            }
            else
            {
                entity.ApprovalStatus = "Pending";
                entity.ApprovedBy = null;
                entity.ApprovedAt = null;
                entity.RejectionReason = null;
            }
        }

        /// <summary>Out-of-stock listings are treated the same as a manual "Mark as Sold" - one mental model, not two.</summary>
        private static void ApplyStockRules(Listing entity)
        {
            if (entity.Quantity <= 0 && entity.Status == "Active")
                entity.Status = "Sold";
        }

        public async System.Threading.Tasks.Task ApproveListingAsync(long listingId, long approverId)
        {
            var entity = await _listingRepository.GetByIdAsync(listingId)
                ?? throw new NotFoundException("Listing", listingId);

            entity.ApprovalStatus = "Approved";
            entity.ApprovedBy = approverId;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.RejectionReason = null;

            _listingRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task RejectListingAsync(long listingId, long approverId, ListingRejectRequest request)
        {
            var entity = await _listingRepository.GetByIdAsync(listingId)
                ?? throw new NotFoundException("Listing", listingId);

            entity.ApprovalStatus = "Rejected";
            entity.ApprovedBy = approverId;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.RejectionReason = request.RejectionReason;

            _listingRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ListingResponse> GetListingByIdAsync(long listingId)
        {
            var entity = await _listingRepository.GetByIdAsync(listingId)
                ?? throw new NotFoundException("Listing", listingId);

            var response = entity.ToResponse();
            var (average, count) = await _reviewRepository.GetRatingSummaryAsync(listingId);
            response.AverageRating = average;
            response.ReviewCount = count;

            return response;
        }

        public async Task<PagedResult<ListingResponse>> GetListingsPagedAsync(ListingFilterRequest request)
        {
            var pagedResult = await _listingRepository.GetPaginatedAsync(request);
            var responses = pagedResult.Data.Select(l => l.ToResponse()).ToList();

            var ratingSummaries = await _reviewRepository.GetRatingSummariesAsync(responses.Select(r => r.ListingId));
            foreach (var response in responses)
            {
                if (ratingSummaries.TryGetValue(response.ListingId, out var summary))
                {
                    response.AverageRating = summary.Average;
                    response.ReviewCount = summary.Count;
                }
            }

            return new PagedResult<ListingResponse>
            {
                Data = responses,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        // ---------------- Listing images ----------------

        public async Task<long> AddImageAsync(long listingId, ListingImageAddRequest request)
        {
            _ = await _listingRepository.GetByIdAsync(listingId)
                ?? throw new NotFoundException("Listing", listingId);

            var entity = request.ToEntity(listingId);
            await _listingImageRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ImageId;
        }

        public async System.Threading.Tasks.Task RemoveImageAsync(long listingId, long imageId)
        {
            var entity = await _listingImageRepository.GetByIdAsync(imageId);
            if (entity == null || entity.ListingId != listingId)
                throw new NotFoundException("ListingImage", imageId);

            _listingImageRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ListingImageResponse>> GetImagesAsync(long listingId)
        {
            var images = await _listingImageRepository.GetByListingIdAsync(listingId);
            return images.Select(i => i.ToResponse()).ToList();
        }

        // ---------------- Favorites ----------------

        public async Task<long> AddFavoriteAsync(long employeeId, FavoriteAddRequest request)
        {
            _ = await _listingRepository.GetByIdAsync(request.ListingId)
                ?? throw new NotFoundException("Listing", request.ListingId);

            var exists = await _favoriteRepository.ExistsAsync(employeeId, request.ListingId);
            if (exists)
                throw new DuplicateException("Favorite", "ListingId", request.ListingId.ToString());

            var entity = request.ToEntity(employeeId);
            await _favoriteRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.FavoriteId;
        }

        public async System.Threading.Tasks.Task RemoveFavoriteAsync(long employeeId, long listingId)
        {
            var entity = await _favoriteRepository.GetAsync(employeeId, listingId)
                ?? throw new NotFoundException("Favorite", listingId);

            _favoriteRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<FavoriteResponse>> GetFavoritesAsync(long employeeId)
        {
            var favorites = await _favoriteRepository.GetByEmployeeIdAsync(employeeId);
            return favorites.Select(f => f.ToResponse()).ToList();
        }

        // ---------------- Conversations ----------------

        public async Task<long> StartConversationAsync(long initiatorEmployeeId, ConversationStartRequest request)
        {
            var listing = await _listingRepository.GetByIdAsync(request.ListingId)
                ?? throw new NotFoundException("Listing", request.ListingId);

            var conversation = new Conversation
            {
                ListingId = listing.ListingId,
                Status = "Open"
            };
            await _conversationRepository.AddAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            var participantIds = new HashSet<long> { listing.SellerId, initiatorEmployeeId };
            foreach (var employeeId in participantIds)
            {
                await _conversationParticipantRepository.AddAsync(new ConversationParticipant
                {
                    ConversationId = conversation.ConversationId,
                    EmployeeId = employeeId
                });
            }
            await _unitOfWork.SaveChangesAsync();

            return conversation.ConversationId;
        }

        public async Task<ConversationResponse> GetConversationByIdAsync(long conversationId)
        {
            var entity = await _conversationRepository.GetByIdAsync(conversationId)
                ?? throw new NotFoundException("Conversation", conversationId);

            var participants = await _conversationParticipantRepository.GetByConversationIdAsync(conversationId);
            return entity.ToResponse(participants);
        }

        public async Task<PagedResult<ConversationResponse>> GetConversationsPagedAsync(long employeeId, PagedRequest request)
        {
            var pagedResult = await _conversationRepository.GetPaginatedForEmployeeAsync(employeeId, request);

            var responses = new List<ConversationResponse>();
            foreach (var conversation in pagedResult.Data)
            {
                var participants = await _conversationParticipantRepository.GetByConversationIdAsync(conversation.ConversationId);
                responses.Add(conversation.ToResponse(participants));
            }

            return new PagedResult<ConversationResponse>
            {
                Data = responses,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        // ---------------- Messages ----------------

        public async Task<long> SendMessageAsync(long conversationId, long senderId, MessageSendRequest request)
        {
            _ = await _conversationRepository.GetByIdAsync(conversationId)
                ?? throw new NotFoundException("Conversation", conversationId);

            var isParticipant = await _conversationParticipantRepository.ExistsAsync(conversationId, senderId);
            if (!isParticipant)
                throw new ForbiddenException("You are not a participant in this conversation.");

            var entity = request.ToEntity(conversationId, senderId);
            await _messageRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var participants = await _conversationParticipantRepository.GetByConversationIdAsync(conversationId);
            var senderName = await GetEmployeeNameAsync(senderId);

            foreach (var participant in participants.Where(p => p.EmployeeId != senderId))
            {
                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = participant.EmployeeId,
                    Title = $"{senderName} sent you a message",
                    Message = request.MessageText,
                    Category = "MarketplaceMessage",
                    RelatedEntityType = "Conversation",
                    RelatedEntityId = conversationId
                });
            }

            return entity.MessageId;
        }

        public async Task<List<MessageResponse>> GetMessagesAsync(long conversationId)
        {
            var messages = await _messageRepository.GetByConversationIdAsync(conversationId);
            return messages.Select(m => m.ToResponse()).ToList();
        }

        // ---------------- Reports ----------------

        public async Task<long> CreateReportAsync(long reportedBy, MarketplaceReportCreateRequest request)
        {
            _ = await _listingRepository.GetByIdAsync(request.ListingId)
                ?? throw new NotFoundException("Listing", request.ListingId);

            var entity = request.ToEntity(reportedBy);
            await _marketplaceReportRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ReportId;
        }

        public async Task<PagedResult<MarketplaceReportResponse>> GetReportsPagedAsync(PagedRequest request)
        {
            var pagedResult = await _marketplaceReportRepository.GetPaginatedAsync(request);

            return new PagedResult<MarketplaceReportResponse>
            {
                Data = pagedResult.Data.Select(r => r.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async System.Threading.Tasks.Task ResolveReportAsync(long reportId, long reviewerId, MarketplaceReportResolveRequest request)
        {
            var entity = await _marketplaceReportRepository.GetByIdAsync(reportId)
                ?? throw new NotFoundException("MarketplaceReport", reportId);

            entity.Status = request.Status;
            entity.ReviewedBy = reviewerId;
            entity.ReviewedAt = DateTime.UtcNow;

            _marketplaceReportRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        // ---------------- Purchases ----------------

        public async Task<long> CreatePurchaseAsync(long buyerId, PurchaseCreateRequest request)
        {
            var listing = await _listingRepository.GetByIdAsync(request.ListingId)
                ?? throw new NotFoundException("Listing", request.ListingId);

            if (listing.SellerId == buyerId)
                throw new ForbiddenException("You cannot buy your own listing.");

            if (listing.Status != "Active" || listing.ApprovalStatus != "Approved")
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(request.ListingId), "This listing is not available for purchase.")
                });
            }

            if (request.Quantity <= 0 || request.Quantity > listing.Quantity)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(request.Quantity), "Requested quantity is unavailable.")
                });
            }

            var purchase = request.ToEntity(buyerId, listing);
            await _purchaseRepository.AddAsync(purchase);

            listing.Quantity -= request.Quantity;
            ApplyStockRules(listing);
            _listingRepository.Update(listing);

            await _unitOfWork.SaveChangesAsync();

            var buyerName = await GetEmployeeNameAsync(buyerId);
            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                EmployeeId = listing.SellerId,
                Title = $"{buyerName} purchased your listing",
                Message = $"{buyerName} bought {request.Quantity} x \"{listing.Title}\".",
                Category = "MarketplacePurchase",
                RelatedEntityType = "Listing",
                RelatedEntityId = listing.ListingId
            });

            return purchase.PurchaseId;
        }

        public async Task<List<PurchaseResponse>> GetPurchasesForEmployeeAsync(long employeeId)
        {
            var purchases = await _purchaseRepository.GetByBuyerIdAsync(employeeId);
            return purchases.Select(p => p.ToResponse()).ToList();
        }

        public async Task<bool> HasPurchasedAsync(long employeeId, long listingId)
        {
            return await _purchaseRepository.ExistsForBuyerAndListingAsync(employeeId, listingId);
        }

        // ---------------- Reviews ----------------

        public async Task<long> CreateReviewAsync(long reviewerId, ReviewCreateRequest request)
        {
            _ = await _listingRepository.GetByIdAsync(request.ListingId)
                ?? throw new NotFoundException("Listing", request.ListingId);

            var hasPurchased = await _purchaseRepository.ExistsForBuyerAndListingAsync(reviewerId, request.ListingId);
            if (!hasPurchased)
                throw new ForbiddenException("You can only review listings you have purchased.");

            var alreadyReviewed = await _reviewRepository.ExistsAsync(reviewerId, request.ListingId);
            if (alreadyReviewed)
                throw new DuplicateException("Review", "ListingId", request.ListingId.ToString());

            var entity = request.ToEntity(reviewerId);
            await _reviewRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ReviewId;
        }

        public async Task<List<ReviewResponse>> GetReviewsForListingAsync(long listingId)
        {
            var reviews = await _reviewRepository.GetByListingIdAsync(listingId);
            return reviews.Select(r => r.ToResponse()).ToList();
        }

        public async Task<long> AddReviewImageAsync(long reviewId, ReviewImageAddRequest request)
        {
            _ = await _reviewRepository.GetByIdAsync(reviewId)
                ?? throw new NotFoundException("Review", reviewId);

            var entity = request.ToEntity(reviewId);
            await _reviewImageRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ImageId;
        }

        public async Task<List<ReviewImageResponse>> GetReviewImagesAsync(long reviewId)
        {
            var images = await _reviewImageRepository.GetByReviewIdAsync(reviewId);
            return images.Select(i => i.ToResponse()).ToList();
        }
    }
}
