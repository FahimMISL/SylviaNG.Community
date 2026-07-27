using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Marketplace.Models;
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
        private readonly IUnitOfWork _unitOfWork;

        public MarketplaceService(
            IListingRepository listingRepository,
            IListingImageRepository listingImageRepository,
            IFavoriteRepository favoriteRepository,
            IConversationRepository conversationRepository,
            IConversationParticipantRepository conversationParticipantRepository,
            IMessageRepository messageRepository,
            IMarketplaceReportRepository marketplaceReportRepository,
            IUnitOfWork unitOfWork)
        {
            _listingRepository = listingRepository;
            _listingImageRepository = listingImageRepository;
            _favoriteRepository = favoriteRepository;
            _conversationRepository = conversationRepository;
            _conversationParticipantRepository = conversationParticipantRepository;
            _messageRepository = messageRepository;
            _marketplaceReportRepository = marketplaceReportRepository;
            _unitOfWork = unitOfWork;
        }

        // ---------------- Listing ----------------

        public async Task<long> CreateListingAsync(long sellerId, ListingCreateRequest request)
        {
            var entity = request.ToEntity(sellerId);
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
            _listingRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
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

            return entity.ToResponse();
        }

        public async Task<PagedResult<ListingResponse>> GetListingsPagedAsync(ListingFilterRequest request)
        {
            var pagedResult = await _listingRepository.GetPaginatedAsync(request);

            return new PagedResult<ListingResponse>
            {
                Data = pagedResult.Data.Select(l => l.ToResponse()).ToList(),
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
    }
}
