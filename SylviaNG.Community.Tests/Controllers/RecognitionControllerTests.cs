using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Recognitions.Commands.RecognitionCreate;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Queries.RecognitionGetAllPaged;
using SylviaNG.Community.Application.Features.Recognitions.Queries.RecognitionGetById;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class RecognitionControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RecognitionController _controller;

    public RecognitionControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new RecognitionController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new RecognitionResponse { RecognitionId = 1, SenderId = 1, RecipientId = 2, RecognitionType = "Peer" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<RecognitionGetByIdQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetPaged_ShouldReturnOkWithPagedResult()
    {
        // Arrange
        var expected = new PagedResult<RecognitionResponse>
        {
            Data = new List<RecognitionResponse> { new() { RecognitionId = 1, SenderId = 1, RecipientId = 2, RecognitionType = "Peer" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<RecognitionGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(new PagedRequest(), null, null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewId()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<RecognitionCreateCommand>(), default)).ReturnsAsync(42L);

        // Act
        var result = await _controller.Create(new RecognitionCreateRequest
        {
            SenderId = 1,
            RecipientId = 2,
            RecognitionType = "Peer"
        });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }
}
