using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Polls.Commands.PollCreate;
using SylviaNG.Community.Application.Features.Polls.Commands.PollVoteCast;
using SylviaNG.Community.Application.Features.Polls.Models;
using SylviaNG.Community.Application.Features.Polls.Queries.PollGetByPostId;
using SylviaNG.Community.Controllers;

namespace SylviaNG.Community.Tests.Controllers;

public class PollControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly PollController _controller;

    public PollControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new PollController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetByPostId_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new PollResponse
        {
            PollId = 1,
            PostId = 10,
            AllowVoteChange = true,
            Options = new List<PollOptionResponse>
            {
                new() { PollOptionId = 5, PollId = 1, OptionText = "Yes", VoteCount = 2 },
                new() { PollOptionId = 6, PollId = 1, OptionText = "No", VoteCount = 1 }
            }
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<PollGetByPostIdQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetByPostId(10);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
        _mediatorMock.Verify(m => m.Send(It.Is<PollGetByPostIdQuery>(q => q.PostId == 10), default), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewPollId()
    {
        // Arrange
        var request = new PollCreateRequest { AllowVoteChange = true, Options = new List<string> { "Yes", "No" } };
        _mediatorMock.Setup(m => m.Send(It.IsAny<PollCreateCommand>(), default)).ReturnsAsync(100L);

        // Act
        var result = await _controller.Create(10, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(100L);
        _mediatorMock.Verify(m => m.Send(It.Is<PollCreateCommand>(c => c.PostId == 10 && c.Request == request), default), Times.Once);
    }

    [Fact]
    public async Task Vote_ShouldReturnOkWithVoteId()
    {
        // Arrange
        var request = new PollVoteRequest { EmployeeId = 2, PollOptionId = 5 };
        _mediatorMock.Setup(m => m.Send(It.IsAny<PollVoteCastCommand>(), default)).ReturnsAsync(50L);

        // Act
        var result = await _controller.Vote(10, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(50L);
        _mediatorMock.Verify(m => m.Send(It.Is<PollVoteCastCommand>(c => c.PostId == 10 && c.Request == request), default), Times.Once);
    }
}
