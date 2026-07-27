using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
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
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new MarketplaceService(
            _listingRepositoryMock.Object,
            _listingImageRepositoryMock.Object,
            _favoriteRepositoryMock.Object,
            _conversationRepositoryMock.Object,
            _conversationParticipantRepositoryMock.Object,
            _messageRepositoryMock.Object,
            _marketplaceReportRepositoryMock.Object,
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

        var result = await _service.CreateListingAsync(10, request);

        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateListingAsync_ShouldDefaultStatusAndApprovalStatus()
    {
        var request = new ListingCreateRequest { ListingType = "Item", Title = "Chair", Category = "Furniture", Price = 20, Currency = "USD" };
        Listing? captured = null;
        _listingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Listing>())).Callback<Listing>(l => captured = l);

        await _service.CreateListingAsync(10, request);

        captured.Should().NotBeNull();
        captured!.SellerId.Should().Be(10);
        captured.Status.Should().Be("Active");
        captured.ApprovalStatus.Should().Be("Pending");
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
}
