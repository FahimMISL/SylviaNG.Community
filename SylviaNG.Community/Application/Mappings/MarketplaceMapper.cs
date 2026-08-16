using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class MarketplaceMapper
    {
        public static Listing ToEntity(this ListingCreateRequest request, long sellerId)
        {
            return new Listing
            {
                SellerId = sellerId,
                ListingType = request.ListingType,
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Condition = request.Condition,
                Price = request.Price,
                Currency = request.Currency,
                Location = request.Location,
                Quantity = request.Quantity
            };
        }

        public static void ApplyUpdate(this Listing entity, ListingUpdateRequest request)
        {
            if (request.ListingType != null) entity.ListingType = request.ListingType;
            if (request.Title != null) entity.Title = request.Title;
            if (request.Description != null) entity.Description = request.Description;
            if (request.Category != null) entity.Category = request.Category;
            if (request.Condition != null) entity.Condition = request.Condition;
            if (request.Price.HasValue) entity.Price = request.Price.Value;
            if (request.Currency != null) entity.Currency = request.Currency;
            if (request.Location != null) entity.Location = request.Location;
            if (request.Quantity.HasValue) entity.Quantity = request.Quantity.Value;
            if (request.Status != null) entity.Status = request.Status;
        }

        public static ListingResponse ToResponse(this Listing entity)
        {
            return new ListingResponse
            {
                ListingId = entity.ListingId,
                SellerId = entity.SellerId,
                ListingType = entity.ListingType,
                Title = entity.Title,
                Description = entity.Description,
                Category = entity.Category,
                Condition = entity.Condition,
                Price = entity.Price,
                Currency = entity.Currency,
                Location = entity.Location,
                Quantity = entity.Quantity,
                Status = entity.Status,
                ApprovalStatus = entity.ApprovalStatus,
                ApprovedBy = entity.ApprovedBy,
                ApprovedAt = entity.ApprovedAt,
                RejectionReason = entity.RejectionReason,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt
            };
        }

        public static ListingImage ToEntity(this ListingImageAddRequest request, long listingId)
        {
            return new ListingImage
            {
                ListingId = listingId,
                ImageUrl = request.ImageUrl,
                DisplayOrder = request.DisplayOrder
            };
        }

        public static ListingImageResponse ToResponse(this ListingImage entity)
        {
            return new ListingImageResponse
            {
                ImageId = entity.ImageId,
                ListingId = entity.ListingId,
                ImageUrl = entity.ImageUrl,
                DisplayOrder = entity.DisplayOrder
            };
        }

        public static Favorite ToEntity(this FavoriteAddRequest request, long employeeId)
        {
            return new Favorite
            {
                EmployeeId = employeeId,
                ListingId = request.ListingId
            };
        }

        public static FavoriteResponse ToResponse(this Favorite entity)
        {
            return new FavoriteResponse
            {
                FavoriteId = entity.FavoriteId,
                EmployeeId = entity.EmployeeId,
                ListingId = entity.ListingId,
                CreatedAt = entity.CreatedAt
            };
        }

        public static ConversationParticipantResponse ToResponse(this ConversationParticipant entity)
        {
            return new ConversationParticipantResponse
            {
                ParticipantId = entity.ParticipantId,
                ConversationId = entity.ConversationId,
                EmployeeId = entity.EmployeeId
            };
        }

        public static ConversationResponse ToResponse(this Conversation entity, List<ConversationParticipant>? participants = null)
        {
            return new ConversationResponse
            {
                ConversationId = entity.ConversationId,
                ListingId = entity.ListingId,
                Status = entity.Status,
                Participants = (participants ?? new List<ConversationParticipant>()).Select(p => p.ToResponse()).ToList()
            };
        }

        public static Message ToEntity(this MessageSendRequest request, long conversationId, long senderId)
        {
            return new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                MessageText = request.MessageText,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };
        }

        public static MessageResponse ToResponse(this Message entity)
        {
            return new MessageResponse
            {
                MessageId = entity.MessageId,
                ConversationId = entity.ConversationId,
                SenderId = entity.SenderId,
                MessageText = entity.MessageText,
                IsRead = entity.IsRead,
                SentAt = entity.SentAt
            };
        }

        public static MarketplaceReport ToEntity(this MarketplaceReportCreateRequest request, long reportedBy)
        {
            return new MarketplaceReport
            {
                ListingId = request.ListingId,
                ReportedBy = reportedBy,
                Reason = request.Reason,
                Status = "Open"
            };
        }

        public static MarketplaceReportResponse ToResponse(this MarketplaceReport entity)
        {
            return new MarketplaceReportResponse
            {
                ReportId = entity.ReportId,
                ListingId = entity.ListingId,
                ReportedBy = entity.ReportedBy,
                Reason = entity.Reason,
                Status = entity.Status,
                ReviewedBy = entity.ReviewedBy,
                ReviewedAt = entity.ReviewedAt
            };
        }

        public static Purchase ToEntity(this PurchaseCreateRequest request, long buyerId, Listing listing)
        {
            return new Purchase
            {
                ListingId = request.ListingId,
                BuyerId = buyerId,
                Quantity = request.Quantity,
                UnitPrice = listing.Price,
                Currency = listing.Currency
            };
        }

        public static PurchaseResponse ToResponse(this Purchase entity)
        {
            return new PurchaseResponse
            {
                PurchaseId = entity.PurchaseId,
                ListingId = entity.ListingId,
                BuyerId = entity.BuyerId,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                Currency = entity.Currency,
                CreatedAt = entity.CreatedAt
            };
        }

        public static Review ToEntity(this ReviewCreateRequest request, long reviewerId)
        {
            return new Review
            {
                ListingId = request.ListingId,
                ReviewerId = reviewerId,
                Rating = request.Rating,
                Comment = request.Comment
            };
        }

        public static ReviewResponse ToResponse(this Review entity)
        {
            return new ReviewResponse
            {
                ReviewId = entity.ReviewId,
                ListingId = entity.ListingId,
                ReviewerId = entity.ReviewerId,
                Rating = entity.Rating,
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt
            };
        }

        public static ReviewImage ToEntity(this ReviewImageAddRequest request, long reviewId)
        {
            return new ReviewImage
            {
                ReviewId = reviewId,
                ImageUrl = request.ImageUrl,
                DisplayOrder = request.DisplayOrder
            };
        }

        public static ReviewImageResponse ToResponse(this ReviewImage entity)
        {
            return new ReviewImageResponse
            {
                ImageId = entity.ImageId,
                ReviewId = entity.ReviewId,
                ImageUrl = entity.ImageUrl,
                DisplayOrder = entity.DisplayOrder
            };
        }
    }
}
