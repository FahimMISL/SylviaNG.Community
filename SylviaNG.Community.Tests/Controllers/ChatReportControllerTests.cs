using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Features.ChatReports.Commands.ChatReportResolve;
using SylviaNG.Community.Application.Features.ChatReports.Models;
using SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportConversationGetForModeration;
using SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportGetAllPaged;
using SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportMessagesGetPagedForModeration;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class ChatReportControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ChatReportController _controller;

    public ChatReportControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ChatReportController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetPaged_ShouldReturnOkWithEnrichedQueueItems()
    {
        // Arrange
        var expected = new PagedResult<ChatReportQueueItemResponse>
        {
            Data = new List<ChatReportQueueItemResponse>
            {
                new() { ReportId = 1, ReporterName = "Reporter", SenderName = "Sender", MessageBodyPreview = "Hi" },
            },
            TotalCount = 1,
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<ChatReportGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(new PagedRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Resolve_ShouldReturnOk()
    {
        // Arrange
        var request = new ChatReportResolveRequest { ReviewedBy = 9, Status = "Resolved" };

        // Act
        var result = await _controller.Resolve(1, request);

        // Assert
        result.Should().BeOfType<OkResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<ChatReportResolveCommand>(c => c.ReportId == 1), default), Times.Once);
    }

    [Fact]
    public async Task GetConversationForModeration_ShouldReturnOkWithConversation()
    {
        // Arrange
        var expected = new ChatConversationResponse { ChatConversationId = 1, Title = "Group chat" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<ChatReportConversationGetForModerationQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetConversationForModeration(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetMessagesForModeration_ShouldReturnOkWithFullThread()
    {
        // Arrange
        var expected = new PagedResult<ChatMessageResponse>
        {
            Data = new List<ChatMessageResponse> { new() { ChatMessageId = 10, Body = "hi" } },
            TotalCount = 1,
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<ChatReportMessagesGetPagedForModerationQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetMessagesForModeration(1, new PagedRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }
}
