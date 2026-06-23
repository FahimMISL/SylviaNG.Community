using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementCreate;
using SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementDelete;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetAll;
using SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetById;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Tests.Controllers;

public class AnnouncementControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AnnouncementController _controller;

    public AnnouncementControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AnnouncementController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithAnnouncements()
    {
        // Arrange
        var expected = new List<AnnouncementResponse>
        {
            new() { AnnouncementId = 1, Title = "Town Hall", Status = AnnouncementStatusEnum.Published },
            new() { AnnouncementId = 2, Title = "Holiday Notice", Status = AnnouncementStatusEnum.Draft }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<AnnouncementGetAllQuery>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithAnnouncement()
    {
        // Arrange
        var expected = new AnnouncementResponse { AnnouncementId = 1, Title = "Town Hall" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<AnnouncementGetByIdQuery>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_WithValidRequest_ShouldReturnCreatedId()
    {
        // Arrange
        var request = new AnnouncementCreateRequest
        {
            Title = "New Announcement",
            SiteId = 1
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<AnnouncementCreateCommand>(), default))
            .ReturnsAsync(1L);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(1L);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnOk()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<AnnouncementDeleteCommand>(), default))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}