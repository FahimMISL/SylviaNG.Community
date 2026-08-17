using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Roles.Commands.RoleCreate;
using SylviaNG.Community.Application.Features.Roles.Models;
using SylviaNG.Community.Application.Features.Roles.Queries.RoleGetAllPaged;
using SylviaNG.Community.Application.Features.Roles.Queries.RoleGetById;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class RoleControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RoleController _controller;

    public RoleControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new RoleController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new RoleResponse { RoleId = 1, Name = "HR" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<RoleGetByIdQuery>(), default)).ReturnsAsync(expected);

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
        var expected = new PagedResult<RoleResponse>
        {
            Data = new List<RoleResponse> { new() { RoleId = 1, Name = "HR" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<RoleGetAllPagedQuery>(), default)).ReturnsAsync(expected);

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
        _mediatorMock.Setup(m => m.Send(It.IsAny<RoleCreateCommand>(), default)).ReturnsAsync(42L);

        // Act
        var result = await _controller.Create(new RoleCreateRequest { Name = "HR" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }
}
