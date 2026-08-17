using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Designations.Commands.DesignationCreate;
using SylviaNG.Community.Application.Features.Designations.Models;
using SylviaNG.Community.Application.Features.Designations.Queries.DesignationGetAllPaged;
using SylviaNG.Community.Application.Features.Designations.Queries.DesignationGetById;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class DesignationControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DesignationController _controller;

    public DesignationControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new DesignationController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new DesignationResponse { DesignationId = 1, Name = "Software Engineer" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<DesignationGetByIdQuery>(), default)).ReturnsAsync(expected);

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
        var expected = new PagedResult<DesignationResponse>
        {
            Data = new List<DesignationResponse> { new() { DesignationId = 1, Name = "Software Engineer" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<DesignationGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(new PagedRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewId()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DesignationCreateCommand>(), default)).ReturnsAsync(42L);

        // Act
        var result = await _controller.Create(new DesignationCreateRequest { Name = "Software Engineer" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }
}
