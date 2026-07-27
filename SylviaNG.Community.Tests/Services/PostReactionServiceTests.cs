using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Features.PostReactions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class PostReactionServiceTests
{
    private readonly Mock<IPostReactionRepository> _reactionRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PostReactionService _service;

    public PostReactionServiceTests()
    {
        _reactionRepositoryMock = new Mock<IPostReactionRepository>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new PostReactionService(_reactionRepositoryMock.Object, _postRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenNoExistingReaction_ShouldCreateNew()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1 });
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync((PostReaction?)null);
        _reactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PostReaction>()))
            .Callback<PostReaction>(x => x.ReactionId = 7);

        // Act
        var result = await _service.AddOrToggleAsync(1, new PostReactionAddRequest { EmployeeId = 2, ReactionType = "Like" });

        // Assert
        result.Should().NotBeNull();
        result!.ReactionId.Should().Be(7);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenSameReactionExists_ShouldToggleOff()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1 });
        var existing = new PostReaction { ReactionId = 5, PostId = 1, EmployeeId = 2, ReactionType = "Like" };
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync(existing);

        // Act
        var result = await _service.AddOrToggleAsync(1, new PostReactionAddRequest { EmployeeId = 2, ReactionType = "Like" });

        // Assert
        result.Should().BeNull();
        _reactionRepositoryMock.Verify(r => r.Delete(existing), Times.Once);
    }
}
