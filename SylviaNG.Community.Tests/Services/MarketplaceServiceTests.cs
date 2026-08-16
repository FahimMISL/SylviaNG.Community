using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Services;

public class MarketplaceServiceTests
{
    private readonly Mock<IListingRepository> _listingRepositoryMock;
    private readonly Mock<IListingImageRepository> _listingImageRepositoryMock;
    private readonly Mock<IFavoriteRepository> _favoriteRepositoryMock;
    private readonly Mock<IConversationRepository> _conversationRepositoryMock;
    private readonly Mock<IConversationParticipantRepository> _conversationParticipantRepositoryMock;
    private readonly Mock<IMessageRepository> _messageRepositoryMock;
    private readonly Mock<IMarketplaceReportRepository> _marketplaceReportRepositoryMock;
    private readonly Mock<IPurchaseRepository> _purchaseRepositoryMock;
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IReviewImageRepository> _reviewImageRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MarketplaceService _service;

    public MarketplaceServiceTests()
    {
        _listingRepositoryMock = new Mock<IListingRepository>();
        _listingImageRepositoryMock = new Mock<IListingImageRepository>();
        _favoriteRepositoryMock = new Mock<IFavoriteRepository>();
        _conversationRepositoryMock = new Mock<IConversationRepository>();
        _conversationParticipantRepositoryMock = new Mock<IConversationParticipantRepository>();
        _messageRepositoryMock = new Mock<IMessageRepository>();
        _marketplaceReportRepositoryMock = new Mock<IMarketplaceReportRepository>();
        _purchaseRepositoryMock = new Mock<IPurchaseRepository>();
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _reviewImageRepositoryMock = new Mock<IReviewImageRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _conversationParticipantRepositoryMock
            .Setup(r => r.GetByConversationIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new List<ConversationParticipant>());

        _reviewRepositoryMock
            .Setup(r => r.GetRatingSummaryAsync(It.IsAny<long>()))
            .ReturnsAsync(((double?)null, 0));

        _reviewRepositoryMock
            .Setup(r => r.GetRatingSummariesAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new Dictionary<long, (double? Average, int Count)>());

        _service = new MarketplaceService(
            _listingRepositoryMock.Object,
            _listingImageRepositoryMock.Object,
            _favoriteRepositoryMock.Object,
            _conversationRepositoryMock.Object,
            _conversationParticipantRepositoryMock.Object,
            _messageRepositoryMock.Object,
            _marketplaceReportRepositoryMock.Object,
            _purchaseRepositoryMock.Object,
            _reviewRepositoryMock.Object,
            _reviewImageRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _notificationServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    // ---------------- Listing ----------------

    [Fact]
    public async System.Threading.Tasks.Task CreateListingAsync_WithValidRequest_ShouldReturnId()
    {
        var request = new ListingCreateRequest
        {
            ListingType = "Item",
            Title = "Old bicycle",
            Category = "Sports",
            Price = 50,
            Currency = "USD"
        };

        _listingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Listing>()))
            .Callback<Listing>(l => l.ListingId = 1);

        var result = await _service.CreateListingAsync(10, false, request);

        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateListingAsync_WhenEmployeeSubmitsForReview_ShouldSetActivePending()
    {
        var request = new ListingCreateRequest { ListingType = "Item", Title = "Chair", Category = "Furniture", Price = 20, Currency = "USD", SaveAsDraft = false };
        Listing? captured = null;
        _listingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Listing>())).Callback<Listing>(l => captured = l);

        await _service.CreateListingAsync(10, false, request);

        captured.Should().NotBeNull();
        captured!.SellerId.Should().Be(10);
        captured.Status.Should().Be("Active");
        captured.ApprovalStatus.Should().Be("Pending");
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateListingAsync_WhenEmployeeSavesAsDraft_ShouldSetDraftStatus()
    {
        var request = new ListingCreateRequest { ListingType = "Item", Title = "Chair", Category = "Furniture", Price = 20, Currency = "USD", SaveAsDraft = true };
        Listing? captured = null;
        _listingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Listing>())).Callback<Listing>(l => captured = l);

        await _service.CreateListingAsync(10, false, request);

        captured!.Status.Should().Be("Draft");
        captured.ApprovalStatus.Should().Be("Draft");
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateListingAsync_WhenHrOrAdmin_ShouldAutoApprove()
    {
        var request = new ListingCreateRequest { ListingType = "Item", Title = "Chair", Category = "Furniture", Price = 20, Currency = "USD", SaveAsDraft = true };
        Listing? captured = null;
        _listingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Listing>())).Callback<Listing>(l => captured = l);

        await _service.CreateListingAsync(10, true, request);

        captured!.Status.Should().Be("Active");
        captured.ApprovalStatus.Should().Be("Approved");
        captured.ApprovedBy.Should().Be(10);
        captured.ApprovedAt.Should().NotBeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetListingByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Listing?)null);

        var act = () => _service.GetListingByIdAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetListingByIdAsync_WhenFound_ShouldReturnResponse()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", Status = "Active", ApprovalStatus = "Approved" };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        var result = await _service.GetListingByIdAsync(1);

        result.ListingId.Should().Be(1);
        result.SellerId.Should().Be(5);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetListingsPagedAsync_ShouldMapPagedResult()
    {
        var pagedResult = new PagedResult<Listing>
        {
            Data = new List<Listing> { new() { ListingId = 1, Title = "Desk", Category = "Furniture", Currency = "USD", Status = "Active", ApprovalStatus = "Approved" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _listingRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<ListingFilterRequest>())).ReturnsAsync(pagedResult);

        var result = await _service.GetListingsPagedAsync(new ListingFilterRequest());

        result.TotalCount.Should().Be(1);
        result.Data.Should().HaveCount(1);
        result.Data[0].ListingId.Should().Be(1);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateListingAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Listing?)null);

        var act = () => _service.UpdateListingAsync(1, 10, false, new ListingUpdateRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateListingAsync_WhenCallerIsNotSellerAndNotHrAdmin_ShouldThrowForbiddenException()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD" };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        var act = () => _service.UpdateListingAsync(1, 99, false, new ListingUpdateRequest { Title = "New title" });

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateListingAsync_WhenCallerIsSeller_ShouldApplyUpdate()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD" };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        await _service.UpdateListingAsync(1, 5, false, new ListingUpdateRequest { Title = "New title" });

        listing.Title.Should().Be("New title");
        _listingRepositoryMock.Verify(r => r.Update(listing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateListingAsync_WhenCallerIsHrAdmin_ShouldApplyUpdateEvenIfNotSeller()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD" };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        await _service.UpdateListingAsync(1, 99, true, new ListingUpdateRequest { Status = "Sold" });

        listing.Status.Should().Be("Sold");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateListingAsync_WhenEmployeeEditsApprovedListing_ShouldRevertToPending()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", Status = "Active", ApprovalStatus = "Approved", ApprovedBy = 42, ApprovedAt = DateTime.UtcNow };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        await _service.UpdateListingAsync(1, 5, false, new ListingUpdateRequest { Title = "New title" });

        listing.Status.Should().Be("Active");
        listing.ApprovalStatus.Should().Be("Pending");
        listing.ApprovedBy.Should().BeNull();
        listing.ApprovedAt.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateListingAsync_WhenEmployeeMarksAsSold_ShouldNotForceReReview()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", Status = "Active", ApprovalStatus = "Approved", ApprovedBy = 42, ApprovedAt = DateTime.UtcNow };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        await _service.UpdateListingAsync(1, 5, false, new ListingUpdateRequest { Status = "Sold" });

        listing.Status.Should().Be("Sold");
        listing.ApprovalStatus.Should().Be("Approved");
        listing.ApprovedBy.Should().Be(42);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateListingAsync_WhenEmployeeReactivatesSoldListing_ShouldRevertToPending()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", Status = "Sold", ApprovalStatus = "Approved", ApprovedBy = 42, ApprovedAt = DateTime.UtcNow };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        await _service.UpdateListingAsync(1, 5, false, new ListingUpdateRequest { Status = "Active" });

        listing.Status.Should().Be("Active");
        listing.ApprovalStatus.Should().Be("Pending");
        listing.ApprovedBy.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateListingAsync_WhenHrOrAdminReactivatesSoldListing_ShouldAutoApprove()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", Status = "Sold", ApprovalStatus = "Approved" };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        await _service.UpdateListingAsync(1, 99, true, new ListingUpdateRequest { Status = "Active" });

        listing.Status.Should().Be("Active");
        listing.ApprovalStatus.Should().Be("Approved");
        listing.ApprovedBy.Should().Be(99);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteListingAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Listing?)null);

        var act = () => _service.DeleteListingAsync(1, 10, false);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteListingAsync_WhenCallerIsNotSellerAndNotHrAdmin_ShouldThrowForbiddenException()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD" };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        var act = () => _service.DeleteListingAsync(1, 99, false);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteListingAsync_WhenCallerIsSeller_ShouldSoftDeleteAndCloseOpenConversations()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", Status = "Active" };
        var openConversation = new Conversation { ConversationId = 100, ListingId = 1, Status = "Open" };
        var closedConversation = new Conversation { ConversationId = 101, ListingId = 1, Status = "Closed" };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);
        _conversationRepositoryMock.Setup(r => r.GetByListingIdAsync(1)).ReturnsAsync(new List<Conversation> { openConversation, closedConversation });

        await _service.DeleteListingAsync(1, 5, false);

        listing.Status.Should().Be("Removed");
        openConversation.Status.Should().Be("Closed");
        _conversationRepositoryMock.Verify(r => r.Update(openConversation), Times.Once);
        _conversationRepositoryMock.Verify(r => r.Update(closedConversation), Times.Never);
        _listingRepositoryMock.Verify(r => r.Update(listing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task ApproveListingAsync_WhenFound_ShouldSetApprovalFields()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", ApprovalStatus = "Pending" };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        await _service.ApproveListingAsync(1, 42);

        listing.ApprovalStatus.Should().Be("Approved");
        listing.ApprovedBy.Should().Be(42);
        listing.ApprovedAt.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task ApproveListingAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Listing?)null);

        var act = () => _service.ApproveListingAsync(1, 42);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task RejectListingAsync_WhenFound_ShouldSetRejectionFields()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", ApprovalStatus = "Pending" };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        await _service.RejectListingAsync(1, 42, new ListingRejectRequest { RejectionReason = "Inappropriate" });

        listing.ApprovalStatus.Should().Be("Rejected");
        listing.RejectionReason.Should().Be("Inappropriate");
        listing.ApprovedBy.Should().Be(42);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task RejectListingAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Listing?)null);

        var act = () => _service.RejectListingAsync(1, 42, new ListingRejectRequest { RejectionReason = "x" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ---------------- Listing images ----------------

    [Fact]
    public async System.Threading.Tasks.Task AddImageAsync_WithValidListing_ShouldReturnId()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Listing { ListingId = 1 });
        _listingImageRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ListingImage>()))
            .Callback<ListingImage>(i => i.ImageId = 7);

        var result = await _service.AddImageAsync(1, new ListingImageAddRequest { ImageUrl = "http://x/y.png" });

        result.Should().Be(7);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddImageAsync_WhenListingNotFound_ShouldThrowNotFoundException()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Listing?)null);

        var act = () => _service.AddImageAsync(1, new ListingImageAddRequest { ImageUrl = "http://x/y.png" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ---------------- Favorites ----------------

    [Fact]
    public async System.Threading.Tasks.Task AddFavoriteAsync_WithValidRequest_ShouldReturnId()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Listing { ListingId = 1 });
        _favoriteRepositoryMock.Setup(r => r.ExistsAsync(10, 1)).ReturnsAsync(false);
        _favoriteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Favorite>()))
            .Callback<Favorite>(f => f.FavoriteId = 3);

        var result = await _service.AddFavoriteAsync(10, new FavoriteAddRequest { ListingId = 1 });

        result.Should().Be(3);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddFavoriteAsync_WhenAlreadyFavorited_ShouldThrowDuplicateException()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Listing { ListingId = 1 });
        _favoriteRepositoryMock.Setup(r => r.ExistsAsync(10, 1)).ReturnsAsync(true);

        var act = () => _service.AddFavoriteAsync(10, new FavoriteAddRequest { ListingId = 1 });

        await act.Should().ThrowAsync<DuplicateException>();
    }

    // ---------------- Conversations ----------------

    [Fact]
    public async System.Threading.Tasks.Task StartConversationAsync_ShouldAddSellerAndBuyerAsParticipants()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5 };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);
        _conversationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Conversation>()))
            .Callback<Conversation>(c => c.ConversationId = 100);

        var result = await _service.StartConversationAsync(10, new ConversationStartRequest { ListingId = 1 });

        result.Should().Be(100);
        _conversationParticipantRepositoryMock.Verify(r => r.AddAsync(It.Is<ConversationParticipant>(p => p.EmployeeId == 5)), Times.Once);
        _conversationParticipantRepositoryMock.Verify(r => r.AddAsync(It.Is<ConversationParticipant>(p => p.EmployeeId == 10)), Times.Once);
    }

    // ---------------- Messages ----------------

    [Fact]
    public async System.Threading.Tasks.Task SendMessageAsync_WhenSenderIsParticipant_ShouldReturnId()
    {
        _conversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Conversation { ConversationId = 1 });
        _conversationParticipantRepositoryMock.Setup(r => r.ExistsAsync(1, 10)).ReturnsAsync(true);
        _messageRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Message>()))
            .Callback<Message>(m => m.MessageId = 55);

        var result = await _service.SendMessageAsync(1, 10, new MessageSendRequest { MessageText = "Hi" });

        result.Should().Be(55);
    }

    [Fact]
    public async System.Threading.Tasks.Task SendMessageAsync_WhenSenderIsNotParticipant_ShouldThrowForbiddenException()
    {
        _conversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Conversation { ConversationId = 1 });
        _conversationParticipantRepositoryMock.Setup(r => r.ExistsAsync(1, 99)).ReturnsAsync(false);

        var act = () => _service.SendMessageAsync(1, 99, new MessageSendRequest { MessageText = "Hi" });

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task SendMessageAsync_ShouldNotifyOtherParticipant()
    {
        _conversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Conversation { ConversationId = 1 });
        _conversationParticipantRepositoryMock.Setup(r => r.ExistsAsync(1, 10)).ReturnsAsync(true);
        _conversationParticipantRepositoryMock.Setup(r => r.GetByConversationIdAsync(1)).ReturnsAsync(new List<ConversationParticipant>
        {
            new() { ConversationId = 1, EmployeeId = 10 },
            new() { ConversationId = 1, EmployeeId = 20 },
        });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Employee { EmployeeId = 10, EmployeeName = "Ayesha Rahman" });
        _messageRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Message>())).Callback<Message>(m => m.MessageId = 55);

        await _service.SendMessageAsync(1, 10, new MessageSendRequest { MessageText = "Is this still available?" });

        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r =>
            r.EmployeeId == 20 &&
            r.Title == "Ayesha Rahman sent you a message" &&
            r.Message == "Is this still available?" &&
            r.Category == "MarketplaceMessage" &&
            r.RelatedEntityType == "Conversation" &&
            r.RelatedEntityId == 1)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task SendMessageAsync_ShouldNotNotifySender()
    {
        _conversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Conversation { ConversationId = 1 });
        _conversationParticipantRepositoryMock.Setup(r => r.ExistsAsync(1, 10)).ReturnsAsync(true);
        _conversationParticipantRepositoryMock.Setup(r => r.GetByConversationIdAsync(1)).ReturnsAsync(new List<ConversationParticipant>
        {
            new() { ConversationId = 1, EmployeeId = 10 },
            new() { ConversationId = 1, EmployeeId = 20 },
        });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Employee { EmployeeId = 10, EmployeeName = "Ayesha Rahman" });

        await _service.SendMessageAsync(1, 10, new MessageSendRequest { MessageText = "Hi" });

        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r => r.EmployeeId == 10)), Times.Never);
    }

    // ---------------- Reports ----------------

    [Fact]
    public async System.Threading.Tasks.Task CreateReportAsync_WithValidListing_ShouldReturnId()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Listing { ListingId = 1 });
        _marketplaceReportRepositoryMock.Setup(r => r.AddAsync(It.IsAny<MarketplaceReport>()))
            .Callback<MarketplaceReport>(r => r.ReportId = 8);

        var result = await _service.CreateReportAsync(10, new MarketplaceReportCreateRequest { ListingId = 1, Reason = "Scam" });

        result.Should().Be(8);
    }

    [Fact]
    public async System.Threading.Tasks.Task ResolveReportAsync_WhenFound_ShouldSetReviewFields()
    {
        var report = new MarketplaceReport { ReportId = 1, ListingId = 1, ReportedBy = 10, Reason = "Scam", Status = "Open" };
        _marketplaceReportRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(report);

        await _service.ResolveReportAsync(1, 42, new MarketplaceReportResolveRequest { Status = "Resolved" });

        report.Status.Should().Be("Resolved");
        report.ReviewedBy.Should().Be(42);
        report.ReviewedAt.Should().NotBeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task ResolveReportAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _marketplaceReportRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((MarketplaceReport?)null);

        var act = () => _service.ResolveReportAsync(1, 42, new MarketplaceReportResolveRequest { Status = "Resolved" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ---------------- Stock rules ----------------

    [Fact]
    public async System.Threading.Tasks.Task UpdateListingAsync_WhenQuantityDropsToZero_ShouldAutoSetStatusSold()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", Status = "Active", ApprovalStatus = "Approved", Quantity = 1 };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        await _service.UpdateListingAsync(1, 5, false, new ListingUpdateRequest { Quantity = 0 });

        listing.Quantity.Should().Be(0);
        listing.Status.Should().Be("Sold");
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateListingAsync_WhenReactivatingWithZeroQuantity_ShouldThrowValidationException()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", Status = "Sold", ApprovalStatus = "Approved", Quantity = 0 };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        var act = () => _service.UpdateListingAsync(1, 5, false, new ListingUpdateRequest { Status = "Active" });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    // ---------------- Purchases ----------------

    [Fact]
    public async System.Threading.Tasks.Task CreatePurchaseAsync_WithValidRequest_ShouldDecrementStockAndReturnId()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Price = 100, Currency = "USD", Status = "Active", ApprovalStatus = "Approved", Quantity = 3 };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);
        _purchaseRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Purchase>())).Callback<Purchase>(p => p.PurchaseId = 9);

        var result = await _service.CreatePurchaseAsync(10, new PurchaseCreateRequest { ListingId = 1, Quantity = 2 });

        result.Should().Be(9);
        listing.Quantity.Should().Be(1);
        listing.Status.Should().Be("Active");
    }

    [Fact]
    public async System.Threading.Tasks.Task CreatePurchaseAsync_WhenQuantityExceedsStock_ShouldThrowValidationException()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Price = 100, Currency = "USD", Status = "Active", ApprovalStatus = "Approved", Quantity = 1 };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        var act = () => _service.CreatePurchaseAsync(10, new PurchaseCreateRequest { ListingId = 1, Quantity = 5 });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreatePurchaseAsync_WhenBuyerIsSeller_ShouldThrowForbiddenException()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Price = 100, Currency = "USD", Status = "Active", ApprovalStatus = "Approved", Quantity = 3 };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        var act = () => _service.CreatePurchaseAsync(5, new PurchaseCreateRequest { ListingId = 1, Quantity = 1 });

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreatePurchaseAsync_WhenListingNotActiveOrApproved_ShouldThrowValidationException()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Price = 100, Currency = "USD", Status = "Active", ApprovalStatus = "Pending", Quantity = 3 };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        var act = () => _service.CreatePurchaseAsync(10, new PurchaseCreateRequest { ListingId = 1, Quantity = 1 });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreatePurchaseAsync_WhenStockReachesZero_ShouldSetListingStatusSold()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Price = 100, Currency = "USD", Status = "Active", ApprovalStatus = "Approved", Quantity = 2 };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);

        await _service.CreatePurchaseAsync(10, new PurchaseCreateRequest { ListingId = 1, Quantity = 2 });

        listing.Quantity.Should().Be(0);
        listing.Status.Should().Be("Sold");
    }

    [Fact]
    public async System.Threading.Tasks.Task CreatePurchaseAsync_ShouldNotifySeller()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Price = 100, Currency = "USD", Status = "Active", ApprovalStatus = "Approved", Quantity = 3 };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Employee { EmployeeId = 10, EmployeeName = "Tanvir Hasan" });

        await _service.CreatePurchaseAsync(10, new PurchaseCreateRequest { ListingId = 1, Quantity = 1 });

        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r =>
            r.EmployeeId == 5 &&
            r.Title == "Tanvir Hasan purchased your listing" &&
            r.Category == "MarketplacePurchase" &&
            r.RelatedEntityType == "Listing" &&
            r.RelatedEntityId == 1)), Times.Once);
    }

    // ---------------- Reviews ----------------

    [Fact]
    public async System.Threading.Tasks.Task CreateReviewAsync_WhenReviewerHasNotPurchased_ShouldThrowForbiddenException()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Listing { ListingId = 1 });
        _purchaseRepositoryMock.Setup(r => r.ExistsForBuyerAndListingAsync(10, 1)).ReturnsAsync(false);

        var act = () => _service.CreateReviewAsync(10, new ReviewCreateRequest { ListingId = 1, Rating = 5 });

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateReviewAsync_WhenReviewerHasPurchased_ShouldReturnId()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Listing { ListingId = 1 });
        _purchaseRepositoryMock.Setup(r => r.ExistsForBuyerAndListingAsync(10, 1)).ReturnsAsync(true);
        _reviewRepositoryMock.Setup(r => r.ExistsAsync(10, 1)).ReturnsAsync(false);
        _reviewRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Review>())).Callback<Review>(r => r.ReviewId = 4);

        var result = await _service.CreateReviewAsync(10, new ReviewCreateRequest { ListingId = 1, Rating = 5, Comment = "Great!" });

        result.Should().Be(4);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateReviewAsync_WhenAlreadyReviewed_ShouldThrowDuplicateException()
    {
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Listing { ListingId = 1 });
        _purchaseRepositoryMock.Setup(r => r.ExistsForBuyerAndListingAsync(10, 1)).ReturnsAsync(true);
        _reviewRepositoryMock.Setup(r => r.ExistsAsync(10, 1)).ReturnsAsync(true);

        var act = () => _service.CreateReviewAsync(10, new ReviewCreateRequest { ListingId = 1, Rating = 5 });

        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetListingByIdAsync_ShouldIncludeAverageRatingAndReviewCount()
    {
        var listing = new Listing { ListingId = 1, SellerId = 5, Title = "Desk", Category = "Furniture", Currency = "USD", Status = "Active", ApprovalStatus = "Approved" };
        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(listing);
        _reviewRepositoryMock.Setup(r => r.GetRatingSummaryAsync(1)).ReturnsAsync((4.5, 2));

        var result = await _service.GetListingByIdAsync(1);

        result.AverageRating.Should().Be(4.5);
        result.ReviewCount.Should().Be(2);
    }
}
