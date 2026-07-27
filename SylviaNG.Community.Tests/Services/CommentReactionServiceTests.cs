using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.CommentReactions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class CommentReactionServiceTests
{
    private readonly Mock<ICommentReactionRepository> _reactionRepositoryMock;
    private readonly Mock<IPostCommentRepository> _commentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CommentReactionService _service;

    public CommentReactionServiceTests()
    {
        _reactionRepositoryMock = new Mock<ICommentReactionRepository>();
        _commentRepositoryMock = new Mock<IPostCommentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new CommentReactionService(_reactionRepositoryMock.Object, _commentRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenCommentNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PostComment?)null);

        // Act
        var act = () => _service.AddOrToggleAsync(1, new CommentReactionAddRequest { EmployeeId = 2, ReactionType = "Like" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddOrToggleAsync_WhenNoExistingReaction_ShouldCreateNew()
    {
        // Arrange
        _commentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new PostComment { CommentId = 1 });
        _reactionRepositoryMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync((CommentReaction?)null);
        _reactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CommentReaction>()))
            .Callback<CommentReaction>(x => x.ReactionId = 9);

        // Act
        var result = await _service.AddOrToggleAsync(1, new CommentReactionAddRequest { EmployeeId = 2, ReactionType = "Love" });

        // Assert
        result.Should().NotBeNull();
        result!.ReactionId.Should().Be(9);
    }
}
