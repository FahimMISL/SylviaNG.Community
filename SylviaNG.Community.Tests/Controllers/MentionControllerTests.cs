using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Mentions.Commands.MentionCreate;
using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.Application.Features.Mentions.Queries.MentionGetAllPaged;
using SylviaNG.Community.Application.Features.Mentions.Queries.MentionGetByEntity;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class MentionControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly MentionController _controller;

    public MentionControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new MentionController(_mediatorMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task GetPaged_ShouldUseCallerEmployeeIdFromCurrentUserService()
    {
        // Arrange
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns(5);
        var expected = new PagedResult<MentionResponse> { Data = new List<MentionResponse>(), TotalCount = 0 };
        _mediatorMock.Setup(m => m.Send(It.IsAny<MentionGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(new PagedRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
        _mediatorMock.Verify(m => m.Send(It.Is<MentionGetAllPagedQuery>(q => q.MentionedEmployeeId == 5), default), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewMentionId()
    {
        // Arrange
        var request = new MentionCreateRequest { MentionedEmployeeId = 1, MentionedBy = 2, EntityType = "Post", EntityId = 10 };
        _mediatorMock.Setup(m => m.Send(It.IsAny<MentionCreateCommand>(), default)).ReturnsAsync(4L);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(4L);
    }

    [Fact]
    public async Task GetByEntity_ShouldReturnOkWithMentionsForThatEntity()
    {
        // Arrange
        var expected = new List<MentionResponse> { new() { MentionId = 1, MentionedEmployeeId = 3, EntityType = "Post", EntityId = 10 } };
        _mediatorMock.Setup(m => m.Send(It.IsAny<MentionGetByEntityQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetByEntity("Post", 10);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
        _mediatorMock.Verify(m => m.Send(It.Is<MentionGetByEntityQuery>(q => q.EntityType == "Post" && q.EntityId == 10), default), Times.Once);
    }
}
