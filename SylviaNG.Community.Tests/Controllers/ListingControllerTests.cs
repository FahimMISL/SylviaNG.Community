using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingApprove;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingCreate;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ListingReject;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Marketplace.Queries.ListingGetAllPaged;
using SylviaNG.Community.Application.Features.Marketplace.Queries.ListingGetById;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class ListingControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly ListingController _controller;

    public ListingControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new ListingController(_mediatorMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        var expected = new ListingResponse { ListingId = 1, Title = "Desk" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<ListingGetByIdQuery>(), default)).ReturnsAsync(expected);

        var result = await _controller.GetById(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetPaged_ShouldReturnOkWithPagedResult()
    {
        var expected = new PagedResult<ListingResponse>
        {
            Data = new List<ListingResponse> { new() { ListingId = 1, Title = "Desk" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<ListingGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        var result = await _controller.GetPaged(new ListingFilterRequest());

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewId()
    {
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns(10);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ListingCreateCommand>(), default)).ReturnsAsync(42L);

        var result = await _controller.Create(new ListingCreateRequest { ListingType = "Item", Title = "Desk", Category = "Furniture", Price = 20, Currency = "USD" });

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }

    [Fact]
    public async Task Approve_ShouldReturnOk()
    {
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns(42);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ListingApproveCommand>(), default)).ReturnsAsync(Unit.Value);

        var result = await _controller.Approve(1);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Reject_ShouldReturnOk()
    {
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns(42);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ListingRejectCommand>(), default)).ReturnsAsync(Unit.Value);

        var result = await _controller.Reject(1, new ListingRejectRequest { RejectionReason = "Spam" });

        result.Should().BeOfType<OkResult>();
    }
}
