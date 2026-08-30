using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.ChatReports.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class ChatReportServiceTests
{
    private readonly Mock<IChatReportRepository> _chatReportRepositoryMock;
    private readonly Mock<IChatMessageRepository> _chatMessageRepositoryMock;
    private readonly Mock<IChatConversationRepository> _chatConversationRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ChatReportService _service;

    public ChatReportServiceTests()
    {
        _chatReportRepositoryMock = new Mock<IChatReportRepository>();
        _chatMessageRepositoryMock = new Mock<IChatMessageRepository>();
        _chatConversationRepositoryMock = new Mock<IChatConversationRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new ChatReportService(
            _chatReportRepositoryMock.Object,
            _chatMessageRepositoryMock.Object,
            _chatConversationRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ResolveAsync_WithValidRequest_ShouldUpdateStatusAndSave()
    {
        // Arrange
        var report = new ChatReport { ChatReportId = 1, Status = "Pending" };
        _chatReportRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(report);

        // Act
        await _service.ResolveAsync(1, new ChatReportResolveRequest { ReviewedBy = 9, Status = "Resolved" });

        // Assert
        report.Status.Should().Be("Resolved");
        report.ReviewedBy.Should().Be(9);
        report.ReviewedAt.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ResolveAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _chatReportRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ChatReport?)null);

        // Act
        var act = () => _service.ResolveAsync(1, new ChatReportResolveRequest { ReviewedBy = 9, Status = "Resolved" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetPaginatedAsync_ShouldEnrichWithConversationMessageAndEmployeeContext()
    {
        // Arrange
        var report = new ChatReport { ChatReportId = 1, ReportedByEmployeeId = 2, ChatConversationId = 5, ChatMessageId = 10, Reason = "Spam", Status = "Pending" };
        var pagedResult = new PagedResult<ChatReport> { Data = new List<ChatReport> { report }, TotalCount = 1, PageNumber = 1, PageSize = 10 };
        _chatReportRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>())).ReturnsAsync(pagedResult);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new ChatConversation { ChatConversationId = 5, Type = ConversationTypeEnum.Group, Title = "Team chat" });
        _chatMessageRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new ChatMessage { ChatMessageId = 10, SenderEmployeeId = 3, Body = "Some message body" });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { EmployeeId = 2, EmployeeName = "Reporter Name" });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(new Employee { EmployeeId = 3, EmployeeName = "Sender Name" });

        // Act
        var result = await _service.GetPaginatedAsync(new PagedRequest());

        // Assert
        var item = result.Data.Should().ContainSingle().Subject;
        item.ReporterName.Should().Be("Reporter Name");
        item.SenderName.Should().Be("Sender Name");
        item.ConversationTitle.Should().Be("Team chat");
        item.MessageBodyPreview.Should().Be("Some message body");
        item.IsMessageDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task GetPaginatedAsync_WhenDirectConversationHasNoTitle_ShouldFallBackToDirectMessageLabel()
    {
        // Arrange
        var report = new ChatReport { ChatReportId = 1, ReportedByEmployeeId = 2, ChatConversationId = 5, ChatMessageId = 10, Reason = "Spam", Status = "Pending" };
        var pagedResult = new PagedResult<ChatReport> { Data = new List<ChatReport> { report }, TotalCount = 1, PageNumber = 1, PageSize = 10 };
        _chatReportRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>())).ReturnsAsync(pagedResult);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new ChatConversation { ChatConversationId = 5, Type = ConversationTypeEnum.Direct, Title = null });
        _chatMessageRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new ChatMessage { ChatMessageId = 10, SenderEmployeeId = 3, Body = "hi" });

        // Act
        var result = await _service.GetPaginatedAsync(new PagedRequest());

        // Assert
        var item = result.Data.Should().ContainSingle().Subject;
        item.ConversationTitle.Should().Be("Direct message");
    }

    [Fact]
    public async Task GetPaginatedAsync_WhenReportedMessageWasDeleted_ShouldShowDeletedPlaceholder()
    {
        // Arrange
        var report = new ChatReport { ChatReportId = 1, ReportedByEmployeeId = 2, ChatConversationId = 5, ChatMessageId = 10, Reason = "Spam", Status = "Pending" };
        var pagedResult = new PagedResult<ChatReport> { Data = new List<ChatReport> { report }, TotalCount = 1, PageNumber = 1, PageSize = 10 };
        _chatReportRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>())).ReturnsAsync(pagedResult);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new ChatConversation { ChatConversationId = 5, Type = ConversationTypeEnum.Direct, Title = null });
        _chatMessageRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new ChatMessage { ChatMessageId = 10, SenderEmployeeId = 3, Body = "hi", IsDeleted = true });

        // Act
        var result = await _service.GetPaginatedAsync(new PagedRequest());

        // Assert
        var item = result.Data.Should().ContainSingle().Subject;
        item.MessageBodyPreview.Should().Be("[message deleted]");
        item.IsMessageDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetPaginatedAsync_WhenReportIsConversationLevel_ShouldShowConversationReportedPlaceholder()
    {
        // Arrange
        var report = new ChatReport { ChatReportId = 1, ReportedByEmployeeId = 2, ChatConversationId = 5, ChatMessageId = null, Reason = "Harassment pattern", Status = "Pending" };
        var pagedResult = new PagedResult<ChatReport> { Data = new List<ChatReport> { report }, TotalCount = 1, PageNumber = 1, PageSize = 10 };
        _chatReportRepositoryMock.Setup(r => r.GetPaginatedAsync(It.IsAny<PagedRequest>())).ReturnsAsync(pagedResult);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new ChatConversation { ChatConversationId = 5, Type = ConversationTypeEnum.Group, Title = "Team chat" });

        // Act
        var result = await _service.GetPaginatedAsync(new PagedRequest());

        // Assert
        var item = result.Data.Should().ContainSingle().Subject;
        item.MessageBodyPreview.Should().Be("[conversation reported]");
        item.SenderName.Should().Be("Unknown");
    }
}
