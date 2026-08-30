using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
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

public class ChatConversationServiceTests
{
    private readonly Mock<IChatConversationRepository> _chatConversationRepositoryMock;
    private readonly Mock<IChatParticipantRepository> _chatParticipantRepositoryMock;
    private readonly Mock<IChatMessageRepository> _chatMessageRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IFileStorageRepository> _fileStorageRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IMessengerBroadcaster> _messengerBroadcasterMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ChatConversationService _service;

    public ChatConversationServiceTests()
    {
        _chatConversationRepositoryMock = new Mock<IChatConversationRepository>();
        _chatParticipantRepositoryMock = new Mock<IChatParticipantRepository>();
        _chatMessageRepositoryMock = new Mock<IChatMessageRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _fileStorageRepositoryMock = new Mock<IFileStorageRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _messengerBroadcasterMock = new Mock<IMessengerBroadcaster>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new ChatConversationService(
            _chatConversationRepositoryMock.Object,
            _chatParticipantRepositoryMock.Object,
            _chatMessageRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _fileStorageRepositoryMock.Object,
            _notificationServiceMock.Object,
            _messengerBroadcasterMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Direct_WhenNoExistingConversation_ShouldCreateAndNotifyOtherParticipant()
    {
        // Arrange
        _chatConversationRepositoryMock.Setup(r => r.GetDirectConversationAsync(1, 2)).ReturnsAsync((ChatConversation?)null);
        _chatConversationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ChatConversation>()))
            .Callback<ChatConversation>(c => c.ChatConversationId = 5);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new ChatConversation { ChatConversationId = 5, Type = ConversationTypeEnum.Direct });
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveByConversationIdAsync(5))
            .ReturnsAsync(new List<ChatParticipant>
            {
                new ChatParticipant { EmployeeId = 1 },
                new ChatParticipant { EmployeeId = 2 }
            });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { EmployeeId = 1, EmployeeName = "Alice" });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { EmployeeId = 2, EmployeeName = "Bob" });

        var request = new ChatConversationCreateRequest
        {
            Type = ConversationTypeEnum.Direct,
            ParticipantEmployeeIds = new List<long> { 2 }
        };

        // Act
        var result = await _service.CreateAsync(request, 1);

        // Assert
        result.Should().Be(5);
        _chatParticipantRepositoryMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<ChatParticipant>>(p => p.Count() == 2)), Times.Once);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(req =>
            req.EmployeeId == 2 && req.Category == "Messenger" && req.RelatedEntityId == 5)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Direct_WhenExistingConversationFound_ShouldReturnExistingIdWithoutCreatingNew()
    {
        // Arrange
        _chatConversationRepositoryMock.Setup(r => r.GetDirectConversationAsync(1, 2))
            .ReturnsAsync(new ChatConversation { ChatConversationId = 9 });

        var request = new ChatConversationCreateRequest
        {
            Type = ConversationTypeEnum.Direct,
            ParticipantEmployeeIds = new List<long> { 2 }
        };

        // Act
        var result = await _service.CreateAsync(request, 1);

        // Assert
        result.Should().Be(9);
        _chatConversationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ChatConversation>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_Group_ShouldMakeCallerAdminAndNotifyEachOtherParticipant()
    {
        // Arrange
        _chatConversationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ChatConversation>()))
            .Callback<ChatConversation>(c => c.ChatConversationId = 7);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(7))
            .ReturnsAsync(new ChatConversation { ChatConversationId = 7, Type = ConversationTypeEnum.Group, Title = "Project Team" });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { EmployeeId = 1, EmployeeName = "Alice" });

        List<ChatParticipant>? addedParticipants = null;
        _chatParticipantRepositoryMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<ChatParticipant>>()))
            .Callback<IEnumerable<ChatParticipant>>(p => addedParticipants = p.ToList());

        var request = new ChatConversationCreateRequest
        {
            Type = ConversationTypeEnum.Group,
            Title = "Project Team",
            ParticipantEmployeeIds = new List<long> { 2, 3 }
        };

        // Act
        var result = await _service.CreateAsync(request, 1);

        // Assert
        result.Should().Be(7);
        addedParticipants.Should().HaveCount(3);
        addedParticipants!.Single(p => p.EmployeeId == 1).IsAdmin.Should().BeTrue();
        addedParticipants!.Where(p => p.EmployeeId != 1).Should().OnlyContain(p => !p.IsAdmin);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetByIdAsync_WhenCallerNotParticipant_ShouldThrowForbiddenException()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 99)).ReturnsAsync(false);

        // Act
        var act = () => _service.GetByIdAsync(1, 99);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenParticipant_ShouldReturnResponseWithParticipants()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 2)).ReturnsAsync(true);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ChatConversation { ChatConversationId = 1, Type = ConversationTypeEnum.Direct });
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveByConversationIdAsync(1))
            .ReturnsAsync(new List<ChatParticipant>
            {
                new ChatParticipant { ChatParticipantId = 1, ChatConversationId = 1, EmployeeId = 2 },
                new ChatParticipant { ChatParticipantId = 2, ChatConversationId = 1, EmployeeId = 3 }
            });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Employee { EmployeeId = 2, EmployeeName = "Bob" });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(new Employee { EmployeeId = 3, EmployeeName = "Carol" });

        // Act
        var result = await _service.GetByIdAsync(1, 2);

        // Assert
        result.ChatConversationId.Should().Be(1);
        result.Participants.Should().HaveCount(2);
        result.Participants.Should().Contain(p => p.EmployeeName == "Bob");
    }

    [Fact]
    public async Task GetByIdAsync_WhenConversationNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.IsActiveParticipantAsync(1, 2)).ReturnsAsync(true);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ChatConversation?)null);

        // Act
        var act = () => _service.GetByIdAsync(1, 2);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetSummaryForEmployeeAsync_ForDirectConversation_ShouldResolveOtherEmployeeAsDisplayName()
    {
        // Arrange
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ChatConversation { ChatConversationId = 1, Type = ConversationTypeEnum.Direct, LastMessageAt = DateTime.UtcNow });
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveAsync(1, 2))
            .ReturnsAsync(new ChatParticipant { EmployeeId = 2, LastReadAt = null, IsMuted = true, IsPinned = false });
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveByConversationIdAsync(1))
            .ReturnsAsync(new List<ChatParticipant>
            {
                new ChatParticipant { EmployeeId = 2 },
                new ChatParticipant { EmployeeId = 3 }
            });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(new Employee { EmployeeId = 3, EmployeeName = "Carol" });
        _chatMessageRepositoryMock.Setup(r => r.GetUnreadCountAsync(1, 2, null)).ReturnsAsync(4);

        // Act
        var result = await _service.GetSummaryForEmployeeAsync(1, 2);

        // Assert
        result.DisplayName.Should().Be("Carol");
        result.OtherEmployeeId.Should().Be(3);
        result.UnreadCount.Should().Be(4);
        result.IsMuted.Should().BeTrue();
    }

    [Fact]
    public async Task MarkReadAsync_ShouldAdvanceWatermarkAndBroadcast()
    {
        // Arrange
        var participant = new ChatParticipant { ChatConversationId = 1, EmployeeId = 2, LastReadAt = null };
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveAsync(1, 2)).ReturnsAsync(participant);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChatConversation { ChatConversationId = 1 });
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveByConversationIdAsync(1)).ReturnsAsync(new List<ChatParticipant> { participant });

        // Act
        await _service.MarkReadAsync(1, 2);

        // Assert
        participant.LastReadAt.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _messengerBroadcasterMock.Verify(b => b.BroadcastMessageReadAsync(1, 2, It.IsAny<DateTime>(), default), Times.Once);
        _messengerBroadcasterMock.Verify(b => b.BroadcastConversationUpdatedAsync(2, It.IsAny<ChatConversationSummaryResponse>(), default), Times.Once);
    }

    [Fact]
    public async Task MarkReadAsync_WhenCallerNotParticipant_ShouldThrowForbiddenException()
    {
        // Arrange
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveAsync(1, 99)).ReturnsAsync((ChatParticipant?)null);

        // Act
        var act = () => _service.MarkReadAsync(1, 99);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task SetMutedAsync_ShouldUpdateOwnParticipantFlagOnly()
    {
        // Arrange
        var participant = new ChatParticipant { ChatConversationId = 1, EmployeeId = 2, IsMuted = false };
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveAsync(1, 2)).ReturnsAsync(participant);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChatConversation { ChatConversationId = 1, Type = ConversationTypeEnum.Group });

        // Act
        await _service.SetMutedAsync(1, 2, true);

        // Assert
        participant.IsMuted.Should().BeTrue();
        _messengerBroadcasterMock.Verify(b => b.BroadcastConversationUpdatedAsync(2, It.IsAny<ChatConversationSummaryResponse>(), default), Times.Once);
    }

    [Fact]
    public async Task SetPinnedAsync_WhenUnpinning_ShouldClearPinnedAt()
    {
        // Arrange
        var participant = new ChatParticipant { ChatConversationId = 1, EmployeeId = 2, IsPinned = true, PinnedAt = DateTime.UtcNow };
        _chatParticipantRepositoryMock.Setup(r => r.GetActiveAsync(1, 2)).ReturnsAsync(participant);
        _chatConversationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ChatConversation { ChatConversationId = 1, Type = ConversationTypeEnum.Group });

        // Act
        await _service.SetPinnedAsync(1, 2, false);

        // Assert
        participant.IsPinned.Should().BeFalse();
        participant.PinnedAt.Should().BeNull();
    }
}
