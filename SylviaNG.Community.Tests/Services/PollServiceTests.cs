using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Polls.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class PollServiceTests
{
    private readonly Mock<IPollRepository> _pollRepositoryMock;
    private readonly Mock<IPollOptionRepository> _pollOptionRepositoryMock;
    private readonly Mock<IPollVoteRepository> _pollVoteRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IFeedBroadcaster> _feedBroadcasterMock;
    private readonly Mock<ILogger<PollService>> _loggerMock;
    private readonly PollService _service;

    public PollServiceTests()
    {
        _pollRepositoryMock = new Mock<IPollRepository>();
        _pollOptionRepositoryMock = new Mock<IPollOptionRepository>();
        _pollVoteRepositoryMock = new Mock<IPollVoteRepository>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _feedBroadcasterMock = new Mock<IFeedBroadcaster>();
        _loggerMock = new Mock<ILogger<PollService>>();
        _service = new PollService(
            _pollRepositoryMock.Object,
            _pollOptionRepositoryMock.Object,
            _pollVoteRepositoryMock.Object,
            _postRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _feedBroadcasterMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldCreatePollAndOptions()
    {
        // Arrange
        var post = new Post { PostId = 1, IsPoll = false };
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        _pollRepositoryMock.Setup(r => r.ExistsByPostIdAsync(1)).ReturnsAsync(false);
        _pollRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Poll>()))
            .Callback<Poll>(p => p.PollId = 100);

        var request = new PollCreateRequest { AllowVoteChange = true, Options = new List<string> { "Yes", "No" } };

        // Act
        var result = await _service.CreateAsync(1, request);

        // Assert
        result.Should().Be(100);
        post.IsPoll.Should().BeTrue();
        _pollOptionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<PollOption>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateAsync_WhenPollAlreadyExists_ShouldThrowDuplicateException()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1 });
        _pollRepositoryMock.Setup(r => r.ExistsByPostIdAsync(1)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(1, new PollCreateRequest { Options = new List<string> { "Yes", "No" } });

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task VoteAsync_WhenPollExpired_ShouldThrowForbiddenException()
    {
        // Arrange
        var poll = new Poll { PollId = 1, PostId = 1, ExpirationDate = DateTime.UtcNow.AddDays(-1) };
        _pollRepositoryMock.Setup(r => r.GetByPostIdAsync(1)).ReturnsAsync(poll);

        // Act
        var act = () => _service.VoteAsync(1, new PollVoteRequest { EmployeeId = 2, PollOptionId = 5 });

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task VoteAsync_WhenNoExistingVote_ShouldCreateNewVote()
    {
        // Arrange
        var poll = new Poll { PollId = 1, PostId = 1, AllowVoteChange = false };
        _pollRepositoryMock.Setup(r => r.GetByPostIdAsync(1)).ReturnsAsync(poll);
        _pollOptionRepositoryMock.Setup(r => r.GetByPollIdAsync(1))
            .ReturnsAsync(new List<PollOption> { new() { PollOptionId = 5, PollId = 1 }, new() { PollOptionId = 6, PollId = 1 } });
        _pollVoteRepositoryMock.Setup(r => r.GetByEmployeeAndOptionsAsync(2, It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync((PollVote?)null);
        _pollVoteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PollVote>()))
            .Callback<PollVote>(v => v.VoteId = 50);

        // Act
        var result = await _service.VoteAsync(1, new PollVoteRequest { EmployeeId = 2, PollOptionId = 5 });

        // Assert
        result.Should().Be(50);
    }

    [Fact]
    public async Task VoteAsync_WhenAlreadyVotedAndChangeNotAllowed_ShouldThrowForbiddenException()
    {
        // Arrange
        var poll = new Poll { PollId = 1, PostId = 1, AllowVoteChange = false };
        _pollRepositoryMock.Setup(r => r.GetByPostIdAsync(1)).ReturnsAsync(poll);
        _pollOptionRepositoryMock.Setup(r => r.GetByPollIdAsync(1))
            .ReturnsAsync(new List<PollOption> { new() { PollOptionId = 5, PollId = 1 }, new() { PollOptionId = 6, PollId = 1 } });
        _pollVoteRepositoryMock.Setup(r => r.GetByEmployeeAndOptionsAsync(2, It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new PollVote { VoteId = 20, PollOptionId = 5, EmployeeId = 2 });

        // Act
        var act = () => _service.VoteAsync(1, new PollVoteRequest { EmployeeId = 2, PollOptionId = 6 });

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task VoteAsync_WhenVoteSucceeds_ShouldBroadcastPollResultsForPost()
    {
        // Arrange
        var poll = new Poll { PollId = 1, PostId = 1, AllowVoteChange = false };
        var options = new List<PollOption> { new() { PollOptionId = 5, PollId = 1 }, new() { PollOptionId = 6, PollId = 1 } };
        _pollRepositoryMock.Setup(r => r.GetByPostIdAsync(1)).ReturnsAsync(poll);
        _pollOptionRepositoryMock.Setup(r => r.GetByPollIdAsync(1)).ReturnsAsync(options);
        _pollVoteRepositoryMock.Setup(r => r.GetByEmployeeAndOptionsAsync(2, It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync((PollVote?)null);
        _pollVoteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PollVote>()))
            .Callback<PollVote>(v => v.VoteId = 50);
        _pollVoteRepositoryMock.Setup(r => r.GetByOptionsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<PollVote> { new() { VoteId = 50, PollOptionId = 5, EmployeeId = 2 } });

        // Act
        await _service.VoteAsync(1, new PollVoteRequest { EmployeeId = 2, PollOptionId = 5 });

        // Assert
        _feedBroadcasterMock.Verify(
            b => b.BroadcastPollResultsAsync(1, It.IsAny<PollResponse>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task VoteAsync_WhenBroadcastThrows_ShouldNotFailTheVote()
    {
        // Arrange
        var poll = new Poll { PollId = 1, PostId = 1, AllowVoteChange = false };
        var options = new List<PollOption> { new() { PollOptionId = 5, PollId = 1 }, new() { PollOptionId = 6, PollId = 1 } };
        _pollRepositoryMock.Setup(r => r.GetByPostIdAsync(1)).ReturnsAsync(poll);
        _pollOptionRepositoryMock.Setup(r => r.GetByPollIdAsync(1)).ReturnsAsync(options);
        _pollVoteRepositoryMock.Setup(r => r.GetByEmployeeAndOptionsAsync(2, It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync((PollVote?)null);
        _pollVoteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PollVote>()))
            .Callback<PollVote>(v => v.VoteId = 50);
        _pollVoteRepositoryMock.Setup(r => r.GetByOptionsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<PollVote> { new() { VoteId = 50, PollOptionId = 5, EmployeeId = 2 } });
        _feedBroadcasterMock.Setup(b => b.BroadcastPollResultsAsync(It.IsAny<long>(), It.IsAny<PollResponse>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("hub unavailable"));

        // Act
        var result = await _service.VoteAsync(1, new PollVoteRequest { EmployeeId = 2, PollOptionId = 5 });

        // Assert
        result.Should().Be(50);
    }
}
