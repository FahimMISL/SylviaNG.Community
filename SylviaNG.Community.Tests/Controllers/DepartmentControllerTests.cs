using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Departments.Commands.DepartmentCreate;
using SylviaNG.Community.Application.Features.Departments.Models;
using SylviaNG.Community.Application.Features.Departments.Queries.DepartmentGetAllPaged;
using SylviaNG.Community.Application.Features.Departments.Queries.DepartmentGetById;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class DepartmentControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DepartmentController _controller;

    public DepartmentControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new DepartmentController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new DepartmentResponse { DepartmentId = 1, Name = "Engineering" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<DepartmentGetByIdQuery>(), default)).ReturnsAsync(expected);

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
        var expected = new PagedResult<DepartmentResponse>
        {
            Data = new List<DepartmentResponse> { new() { DepartmentId = 1, Name = "Engineering" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<DepartmentGetAllPagedQuery>(), default)).ReturnsAsync(expected);

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
        _mediatorMock.Setup(m => m.Send(It.IsAny<DepartmentCreateCommand>(), default)).ReturnsAsync(42L);

        // Act
        var result = await _controller.Create(new DepartmentCreateRequest { Name = "Engineering" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }
}
