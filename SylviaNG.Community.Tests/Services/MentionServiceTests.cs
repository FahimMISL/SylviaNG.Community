using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class MentionServiceTests
{
    private readonly Mock<IMentionRepository> _mentionRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IPostCommentRepository> _postCommentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly MentionService _service;

    public MentionServiceTests()
    {
        _mentionRepositoryMock = new Mock<IMentionRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _postCommentRepositoryMock = new Mock<IPostCommentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _notificationServiceMock = new Mock<INotificationService>();
        _notificationServiceMock.Setup(n => n.CreateAsync(It.IsAny<NotificationCreateRequest>())).ReturnsAsync(1L);
        _service = new MentionService(
            _mentionRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _postCommentRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        _mentionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Mention>()))
            .Callback<Mention>(m => m.MentionId = 4);

        var request = new MentionCreateRequest { MentionedEmployeeId = 1, MentionedBy = 2, EntityType = "Post", EntityId = 10 };

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(4);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldNotifyTheMentionedEmployee()
    {
        // Arrange
        var request = new MentionCreateRequest { MentionedEmployeeId = 1, MentionedBy = 2, EntityType = "Post", EntityId = 10 };

        // Act
        await _service.CreateAsync(request);

        // Assert
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(
            r => r.EmployeeId == 1 && r.Category == "Mention" && r.RelatedEntityId == 10)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ForCommentEntity_ShouldUseCommentMentionCategory()
    {
        // Arrange
        _postCommentRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new PostComment { CommentId = 10, PostId = 99 });
        var request = new MentionCreateRequest { MentionedEmployeeId = 1, MentionedBy = 2, EntityType = "PostComment", EntityId = 10 };

        // Act
        await _service.CreateAsync(request);

        // Assert - points at the parent Post (99), not the comment itself (10), since
        // there's no standalone comment page to navigate to.
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(
            r => r.Category == "CommentMention" && r.RelatedEntityType == "Post" && r.RelatedEntityId == 99)), Times.Once);
    }

    [Fact]
    public async Task CreateMentionsAsync_WithNullList_ShouldDoNothing()
    {
        // Act
        await _service.CreateMentionsAsync("Post", 10, 2, null);

        // Assert
        _mentionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Mention>()), Times.Never);
    }

    [Fact]
    public async Task CreateMentionsAsync_ShouldSkipSelfMention()
    {
        // Act
        await _service.CreateMentionsAsync("Post", 10, 2, new List<long> { 2 });

        // Assert
        _mentionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Mention>()), Times.Never);
    }

    [Fact]
    public async Task CreateMentionsAsync_ShouldDedupeDuplicateIds()
    {
        // Act
        await _service.CreateMentionsAsync("Post", 10, 2, new List<long> { 1, 1, 1 });

        // Assert
        _mentionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Mention>()), Times.Once);
    }

    [Fact]
    public async Task CreateMentionsAsync_ShouldCreateOneMentionAndOneNotificationPerUniqueEmployee()
    {
        // Act
        await _service.CreateMentionsAsync("Post", 10, 2, new List<long> { 1, 3 });

        // Assert
        _mentionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Mention>()), Times.Exactly(2));
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r => r.EmployeeId == 1)), Times.Once);
        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<NotificationCreateRequest>(r => r.EmployeeId == 3)), Times.Once);
    }

    [Fact]
    public async Task GetByEntityAsync_ShouldReturnMappedMentionsForThatEntity()
    {
        // Arrange
        _mentionRepositoryMock.Setup(r => r.GetByEntityAsync("Post", 10)).ReturnsAsync(new List<Mention>
        {
            new() { MentionId = 1, MentionedEmployeeId = 3, EntityType = "Post", EntityId = 10 },
            new() { MentionId = 2, MentionedEmployeeId = 4, EntityType = "Post", EntityId = 10 },
        });

        // Act
        var result = await _service.GetByEntityAsync("Post", 10);

        // Assert
        result.Should().HaveCount(2);
        result.Select(r => r.MentionedEmployeeId).Should().BeEquivalentTo(new[] { 3L, 4L });
    }

    [Fact]
    public async Task GetPaginatedForEmployeeAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var pagedResult = new PagedResult<Mention>
        {
            Data = new List<Mention> { new() { MentionId = 1, MentionedEmployeeId = 1 } },
            TotalCount = 1
        };
        _mentionRepositoryMock.Setup(r => r.GetPaginatedForEmployeeAsync(1, It.IsAny<PagedRequest>())).ReturnsAsync(pagedResult);

        // Act
        var result = await _service.GetPaginatedForEmployeeAsync(1, new PagedRequest());

        // Assert
        result.Data.Should().ContainSingle(m => m.MentionId == 1);
    }
}
