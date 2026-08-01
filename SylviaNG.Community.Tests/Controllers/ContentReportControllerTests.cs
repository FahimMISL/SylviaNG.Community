using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportCreate;
using SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportResolve;
using SylviaNG.Community.Application.Features.ContentReports.Models;
using SylviaNG.Community.Application.Features.ContentReports.Queries.ContentReportGetAllPaged;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class ContentReportControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ContentReportController _controller;

    public ContentReportControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ContentReportController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewReportId()
    {
        // Arrange
        var request = new ContentReportCreateRequest { ReportedBy = 2, PostId = 1, Reason = "Spam" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<ContentReportCreateCommand>(), default)).ReturnsAsync(8L);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(8L);
    }

    [Fact]
    public async Task GetPaged_ShouldReturnOkWithEnrichedQueueItems()
    {
        // Arrange
        var expected = new PagedResult<ContentReportQueueItemResponse>
        {
            Data = new List<ContentReportQueueItemResponse>
            {
                new() { ReportId = 1, ReporterName = "Reporter", PostAuthorName = "Author", PostContentPreview = "Hi" },
            },
            TotalCount = 1,
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<ContentReportGetAllPagedQuery>(), default)).ReturnsAsync(expected);

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
        var request = new ContentReportResolveRequest { ReviewedBy = 9, Status = "Resolved" };

        // Act
        var result = await _controller.Resolve(1, request);

        // Assert
        result.Should().BeOfType<OkResult>();
        _mediatorMock.Verify(m => m.Send(It.Is<ContentReportResolveCommand>(c => c.ReportId == 1), default), Times.Once);
    }
}
