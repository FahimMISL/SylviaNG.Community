using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Hubs;
using SylviaNG.Community.Infrastructure.Services;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Infrastructure.Services;

public class SignalRNotificationBroadcasterTests
{
    private readonly Mock<IHubContext<NotificationHub, INotificationClient>> _hubContextMock;
    private readonly Mock<IHubClients<INotificationClient>> _hubClientsMock;
    private readonly Mock<INotificationClient> _clientProxyMock;
    private readonly SignalRNotificationBroadcaster _broadcaster;

    public SignalRNotificationBroadcasterTests()
    {
        _hubContextMock = new Mock<IHubContext<NotificationHub, INotificationClient>>();
        _hubClientsMock = new Mock<IHubClients<INotificationClient>>();
        _clientProxyMock = new Mock<INotificationClient>();

        _hubContextMock.Setup(h => h.Clients).Returns(_hubClientsMock.Object);

        _broadcaster = new SignalRNotificationBroadcaster(_hubContextMock.Object);
    }

    [Fact]
    public async Task BroadcastAsync_ShouldSendToNotificationEmployeeId()
    {
        // Arrange
        var notification = new NotificationResponse { NotificationId = 1, EmployeeId = 42, Title = "Welcome" };
        _hubClientsMock.Setup(c => c.User("42")).Returns(_clientProxyMock.Object);

        // Act
        await _broadcaster.BroadcastAsync(notification);

        // Assert
        _hubClientsMock.Verify(c => c.User("42"), Times.Once);
        _clientProxyMock.Verify(c => c.ReceiveNotification(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BroadcastUnreadCountAsync_ShouldSendToGivenEmployeeId()
    {
        // Arrange
        _hubClientsMock.Setup(c => c.User("7")).Returns(_clientProxyMock.Object);

        // Act
        await _broadcaster.BroadcastUnreadCountAsync(7, 5);

        // Assert
        _hubClientsMock.Verify(c => c.User("7"), Times.Once);
        _clientProxyMock.Verify(c => c.ReceiveUnreadCount(5, It.IsAny<CancellationToken>()), Times.Once);
    }
}
