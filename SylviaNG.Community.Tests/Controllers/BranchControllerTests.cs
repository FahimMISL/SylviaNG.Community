using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Branches.Commands.BranchCreate;
using SylviaNG.Community.Application.Features.Branches.Models;
using SylviaNG.Community.Application.Features.Branches.Queries.BranchGetAllPaged;
using SylviaNG.Community.Application.Features.Branches.Queries.BranchGetById;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class BranchControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly BranchController _controller;

    public BranchControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new BranchController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new BranchResponse { BranchId = 1, Name = "Head Office" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<BranchGetByIdQuery>(), default)).ReturnsAsync(expected);

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
        var expected = new PagedResult<BranchResponse>
        {
            Data = new List<BranchResponse> { new() { BranchId = 1, Name = "Head Office" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<BranchGetAllPagedQuery>(), default)).ReturnsAsync(expected);

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
        _mediatorMock.Setup(m => m.Send(It.IsAny<BranchCreateCommand>(), default)).ReturnsAsync(42L);

        // Act
        var result = await _controller.Create(new BranchCreateRequest { Name = "Head Office" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }
}
