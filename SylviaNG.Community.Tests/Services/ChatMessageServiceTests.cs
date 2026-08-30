using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class ChatMessageServiceTests
{
    private readonly Mock<IChatMessageRepository> _chatMessageRepositoryMock;
    private readonly Mock<IChatMessageAttachmentRepository> _chatMessageAttachmentRepositoryMock;
    private readonly Mock<IChatMessageReactionRepository> _chatMessageReactionRepositoryMock;
    private readonly Mock<IChatConversationRepository> _chatConversationRepositoryMock;
    private readonly Mock<IChatParticipantRepository> _chatParticipantRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IFileStorageRepository> _fileStorageRepositoryMock;
    private readonly Mock<IChatReportRepository> _chatReportRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IEmployeeKeycloakAccountRepository> _employeeKeycloakAccountRepositoryMock;
    private readonly Mock<IChatConversationService> _chatConversationServiceMock;
    private readonly Mock<IMessengerBroadcaster> _messengerBroadcasterMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ChatMessageService _service;

    public ChatMessageServiceTests()
    {
        _chatMessageRepositoryMock = new Mock<IChatMessageRepository>();
        _chatMessageAttachmentRepositoryMock = new Mock<IChatMessageAttachmentRepository>();
        _chatMessageReactionRepositoryMock = new Mock<IChatMessageReactionRepository>();
        _chatConversationRepositoryMock = new Mock<IChatConversationRepository>();
        _chatParticipantRepositoryMock = new Mock<IChatParticipantRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _fileStorageRepositoryMock = new Mock<IFileStorageRepository>();
        _chatReportRepositoryMock = new Mock<IChatReportRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _employeeKeycloakAccountRepositoryMock = new Mock<IEmployeeKeycloakAccountRepository>();
        _chatConversationServiceMock = new Mock<IChatConversationService>();
        _messengerBroadcasterMock = new Mock<IMessengerBroadcaster>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new ChatMessageService(
            _chatMessageRepositoryMock.Object,
            _chatMessageAttachmentRepositoryMock.Object,
            _chatMessageReactionRepositoryMock.Object,
            _chatConversationRepositoryMock.Object,
            _chatParticipantRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _fileStorageRepositoryMock.Object,
            _chatReportRepositoryMock.Object,
            _notificationServiceMock.Object,
            _employeeKeycloakAccountRepositoryMock.Object,
            _chatConversationServiceMock.Object,
            _messengerBroadcasterMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task SendAsync_WhenCallerNotParticipant_ShouldThrowForbiddenException()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 99)).ReturnsAsync(false);

        // Act
        var act = () => _service.SendAsync(1, new ChatMessageSendRequest { Body = "hi" }, 99);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _chatMessageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ChatMessage>()), Times.Never);
    }

    [Fact]
    public async Task SendAsync_WithValidRequest_ShouldPersistUpdateConversationAndNotifyOtherParticipantsOnly()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 2)).ReturnsAsync(true);
        var conversation = new ChatConversation { ChatConversationId = 1, Type = ConversationTypeEnum.Direct };
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(conversation);
        _chatMessageRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ChatMessage>()))
            .Callback<ChatMessage>(m => m.ChatMessageId = 42);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { EmployeeId = 2, EmployeeName = "Alice" });
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveByConversationIdAsync(1))
            .ReturnsAsync(new List<ChatParticipant>
            {
                new ChatParticipant { EmployeeId = 2 },
                new ChatParticipant { EmployeeId = 3 }
            });
        _chatConversationServiceMock.Setup(s => s.GetSummaryForEmployeeAsync(1, It.IsAny<long>()))
            .ReturnsAsync(new ChatConversationSummaryResponse { ChatConversationId = 1 });

        // Act
        var result = await _service.SendAsync(1, new ChatMessageSendRequest { Body = "Hello there" }, 2);

        // Assert
        result.ChatMessageId.Should().Be(42);
        result.SenderName.Should().Be("Alice");
        conversation.LastMessagePreview.Should().Be("Hello there");
        conversation.LastMessageAt.Should().NotBeNull();
        // Two phases: persist the message first (to get its id for any attachments), then the
        // conversation's LastMessageAt/Preview update - see ChatMessageService.SendAsync.
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));

        // Only the other participant (3) is notified - not the sender (2).
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(req => req.EmployeeId == 3)), Times.Once);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(req => req.EmployeeId == 2)), Times.Never);

        // Every active participant (including the sender) gets a ConversationUpdated push
        // so their own inbox reorders too.
        _messengerBroadcasterMock.Verify(b => b.BroadcastConversationUpdatedAsync(2, It.IsAny<ChatConversationSummaryResponse>(), default), Times.Once);
        _messengerBroadcasterMock.Verify(b => b.BroadcastConversationUpdatedAsync(3, It.IsAny<ChatConversationSummaryResponse>(), default), Times.Once);
        _messengerBroadcasterMock.Verify(b => b.BroadcastMessageAsync(1, It.IsAny<ChatMessageResponse>(), default), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithLongBody_ShouldTruncateConversationPreview()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 2)).ReturnsAsync(true);
        var conversation = new ChatConversation { ChatConversationId = 1 };
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(conversation);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { EmployeeId = 2, EmployeeName = "Alice" });
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveByConversationIdAsync(1)).ReturnsAsync(new List<ChatParticipant>());

        var longBody = new string('a', 150);

        // Act
        await _service.SendAsync(1, new ChatMessageSendRequest { Body = longBody }, 2);

        // Assert
        conversation.LastMessagePreview.Should().HaveLength(123); // 120 chars + "..."
        conversation.LastMessagePreview.Should().EndWith("...");
    }

    [Fact]
    public async Task GetPagedAsync_WhenCallerNotParticipant_ShouldThrowForbiddenException()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 99)).ReturnsAsync(false);

        // Act
        var act = () => _service.GetPagedAsync(1, 99, new PagedRequest());

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task GetPagedAsync_WhenParticipant_ShouldReturnMappedMessages()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 2)).ReturnsAsync(true);
        var pagedResult = new PagedResult<ChatMessage>
        {
            Data = new List<ChatMessage> { new ChatMessage { ChatMessageId = 1, ChatConversationId = 1, SenderEmployeeId = 2, Body = "hi" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _chatMessageRepositoryMock.Setup(r => r.GetByConversationPagedAsync(1, It.IsAny<PagedRequest>())).ReturnsAsync(pagedResult);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { EmployeeId = 2, EmployeeName = "Alice" });
        _chatMessageAttachmentRepositoryMock.Setup(r => r.GetByMessageIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<ChatMessageAttachment>());
        _chatMessageReactionRepositoryMock.Setup(r => r.GetByMessageIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<ChatMessageReaction>());

        // Act
        var result = await _service.GetPagedAsync(1, 2, new PagedRequest());

        // Assert
        var item = result.Data.Should().ContainSingle().Subject;
        item.SenderName.Should().Be("Alice");
        item.Body.Should().Be("hi");
        item.Attachments.Should().BeEmpty();
        item.Reactions.Should().BeEmpty();
    }

    [Fact]
    public async Task SendAsync_WithImageAttachment_ShouldCreateAttachmentAndUseFriendlyPreview()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 2)).ReturnsAsync(true);
        var conversation = new ChatConversation { ChatConversationId = 1 };
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(conversation);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { EmployeeId = 2, EmployeeName = "Alice" });
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveByConversationIdAsync(1)).ReturnsAsync(new List<ChatParticipant>());
        _fileStorageRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(new FileStorage
        {
            FileId = 99,
            OriginalFileName = "photo.png",
            StoragePath = "uploads/Messenger/2026-08/abc.png",
            MimeType = "image/png",
            FileSize = 1024
        });
        _chatMessageRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ChatMessage>()))
            .Callback<ChatMessage>(m => m.ChatMessageId = 55);

        var request = new ChatMessageSendRequest
        {
            MessageType = MessageTypeEnum.Attachment,
            Attachments = new List<ChatMessageAttachmentRequest>
            {
                new ChatMessageAttachmentRequest { FileStorageId = 99, AttachmentType = ChatAttachmentTypeEnum.Image }
            }
        };

        // Act
        var result = await _service.SendAsync(1, request, 2);

        // Assert
        result.Attachments.Should().ContainSingle();
        result.Attachments[0].OriginalFileName.Should().Be("photo.png");
        conversation.LastMessagePreview.Should().Contain("Photo");
        _chatMessageAttachmentRepositoryMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<ChatMessageAttachment>>(a => a.Count() == 1)), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithUnknownAttachmentFileId_ShouldThrowNotFoundException()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 2)).ReturnsAsync(true);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChatConversation { ChatConversationId = 1 });
        _fileStorageRepositoryMock.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((FileStorage?)null);

        var request = new ChatMessageSendRequest
        {
            MessageType = MessageTypeEnum.Attachment,
            Attachments = new List<ChatMessageAttachmentRequest>
            {
                new ChatMessageAttachmentRequest { FileStorageId = 404, AttachmentType = ChatAttachmentTypeEnum.File }
            }
        };

        // Act
        var act = () => _service.SendAsync(1, request, 2);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _chatMessageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ChatMessage>()), Times.Never);
    }

    [Fact]
    public async Task ReactAsync_WhenNoExistingReaction_ShouldAddAndNotifySender()
    {
        // Arrange
        var message = new ChatMessage { ChatMessageId = 10, ChatConversationId = 1, SenderEmployeeId = 3 };
        _chatMessageRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(message);
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 2)).ReturnsAsync(true);
        _chatMessageReactionRepositoryMock.Setup(r => r.GetAsync(10, 2)).ReturnsAsync((ChatMessageReaction?)null);
        _chatMessageReactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ChatMessageReaction>()))
            .Callback<ChatMessageReaction>(r => r.ChatMessageReactionId = 77);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { EmployeeId = 2, EmployeeName = "Bob" });

        // Act
        var result = await _service.ReactAsync(10, ReactionTypeEnum.Love, 2);

        // Assert
        result.Should().NotBeNull();
        result!.ReactionType.Should().Be(ReactionTypeEnum.Love);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(req => req.EmployeeId == 3)), Times.Once);
        _messengerBroadcasterMock.Verify(b => b.BroadcastMessageReactedAsync(1, 10, 2, ReactionTypeEnum.Love, default), Times.Once);
    }

    [Fact]
    public async Task ReactAsync_WhenReactingWithSameTypeAgain_ShouldToggleOffAndReturnNull()
    {
        // Arrange
        var message = new ChatMessage { ChatMessageId = 10, ChatConversationId = 1, SenderEmployeeId = 3 };
        _chatMessageRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(message);
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 2)).ReturnsAsync(true);
        var existing = new ChatMessageReaction { ChatMessageReactionId = 5, ChatMessageId = 10, EmployeeId = 2, ReactionType = ReactionTypeEnum.Like };
        _chatMessageReactionRepositoryMock.Setup(r => r.GetAsync(10, 2)).ReturnsAsync(existing);

        // Act
        var result = await _service.ReactAsync(10, ReactionTypeEnum.Like, 2);

        // Assert
        result.Should().BeNull();
        _chatMessageReactionRepositoryMock.Verify(r => r.Delete(existing), Times.Once);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Never);
        _messengerBroadcasterMock.Verify(b => b.BroadcastMessageReactedAsync(1, 10, 2, null, default), Times.Once);
    }

    [Fact]
    public async Task ReactAsync_WhenCallerNotParticipant_ShouldThrowForbiddenException()
    {
        // Arrange
        var message = new ChatMessage { ChatMessageId = 10, ChatConversationId = 1, SenderEmployeeId = 3 };
        _chatMessageRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(message);
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 99)).ReturnsAsync(false);

        // Act
        var act = () => _service.ReactAsync(10, ReactionTypeEnum.Like, 99);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMappedMessages()
    {
        // Arrange
        var pagedResult = new PagedResult<ChatMessage>
        {
            Data = new List<ChatMessage> { new ChatMessage { ChatMessageId = 1, ChatConversationId = 1, SenderEmployeeId = 2, Body = "found it" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _chatMessageRepositoryMock.Setup(r => r.SearchAsync(2, "found", It.IsAny<PagedRequest>())).ReturnsAsync(pagedResult);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { EmployeeId = 2, EmployeeName = "Alice" });
        _chatMessageAttachmentRepositoryMock.Setup(r => r.GetByMessageIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<ChatMessageAttachment>());
        _chatMessageReactionRepositoryMock.Setup(r => r.GetByMessageIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new List<ChatMessageReaction>());

        // Act
        var result = await _service.SearchAsync(2, "found", new PagedRequest());

        // Assert
        var item = result.Data.Should().ContainSingle().Subject;
        item.Body.Should().Be("found it");
    }

    [Fact]
    public async Task ReportAsync_WhenCallerNotParticipant_ShouldThrowForbiddenException()
    {
        // Arrange
        var message = new ChatMessage { ChatMessageId = 10, ChatConversationId = 1, SenderEmployeeId = 3 };
        _chatMessageRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(message);
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 99)).ReturnsAsync(false);

        // Act
        var act = () => _service.ReportAsync(10, "Spam", 99);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _chatReportRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ChatReport>()), Times.Never);
    }

    [Fact]
    public async Task ReportAsync_WithValidRequest_ShouldPersistReportAndNotifyEveryHrAdmin()
    {
        // Arrange
        var message = new ChatMessage { ChatMessageId = 10, ChatConversationId = 1, SenderEmployeeId = 3 };
        _chatMessageRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(message);
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 2)).ReturnsAsync(true);
        _chatReportRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ChatReport>()))
            .Callback<ChatReport>(r => r.ChatReportId = 500);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { EmployeeId = 2, EmployeeName = "Bob" });
        _employeeKeycloakAccountRepositoryMock
            .Setup(r => r.GetEmployeeIdsByRolesAsync(It.Is<IEnumerable<string>>(roles => roles.Contains("HR") && roles.Contains("Admin"))))
            .ReturnsAsync(new List<long> { 100, 101 });

        // Act
        await _service.ReportAsync(10, "Spam", 2);

        // Assert
        _chatReportRepositoryMock.Verify(r => r.AddAsync(It.Is<ChatReport>(rep =>
            rep.ChatConversationId == 1 && rep.ChatMessageId == 10 && rep.ReportedByEmployeeId == 2 && rep.Status == "Pending")), Times.Once);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(req =>
            req.EmployeeId == 100 && req.RelatedEntityType == "ChatReport" && req.RelatedEntityId == 500)), Times.Once);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(req =>
            req.EmployeeId == 101 && req.RelatedEntityType == "ChatReport" && req.RelatedEntityId == 500)), Times.Once);
    }
}
