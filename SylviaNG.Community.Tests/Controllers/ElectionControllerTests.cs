using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionCreate;
using SylviaNG.Community.Application.Features.Elections.Commands.ElectionVoteCast;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetAllPaged;
using SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetById;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class ElectionControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly ElectionController _controller;

    public ElectionControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _controller = new ElectionController(_mediatorMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new ElectionResponse { ElectionId = 1, Title = "Employee of the Year" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<ElectionGetByIdQuery>(), default)).ReturnsAsync(expected);

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
        var expected = new PagedResult<ElectionResponse>
        {
            Data = new List<ElectionResponse> { new() { ElectionId = 1, Title = "Employee of the Year" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<ElectionGetAllPagedQuery>(), default)).ReturnsAsync(expected);

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
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns(99);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ElectionCreateCommand>(), default)).ReturnsAsync(42L);

        // Act
        var result = await _controller.Create(new ElectionCreateRequest { Title = "Employee of the Year" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }

    [Fact]
    public async Task CastVote_WhenAuthenticated_ShouldReturnOkWithNewIds()
    {
        // Arrange
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns(5);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ElectionVoteCastCommand>(), default)).ReturnsAsync(new List<long> { 7L });

        // Act
        var result = await _controller.CastVote(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new List<long> { 7L });
    }

    [Fact]
    public async Task CastVote_WhenNoEmployeeId_ShouldThrowUnauthorizedException()
    {
        // Arrange
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns((long?)null);

        // Act
        var act = () => _controller.CastVote(1, new ElectionVoteCastRequest { CandidateIds = new List<long> { 10 } });

        // Assert
        await act.Should().ThrowAsync<SylviaNG.Community.Application.Common.Exceptions.UnauthorizedException>();
    }

    [Fact]
    public async Task GetEligible_WhenAuthenticated_ShouldReturnOkWithResult()
    {
        // Arrange
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns(5);
        var expected = new List<ElectionEligibleResponse> { new() { ElectionId = 1, Title = "Employee of the Year" } };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetEligible.ElectionGetEligibleQuery>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetEligible();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetEligible_WhenNoEmployeeId_ShouldThrowUnauthorizedException()
    {
        // Arrange
        _currentUserServiceMock.Setup(c => c.EmployeeId).Returns((long?)null);

        // Act
        var act = () => _controller.GetEligible();

        // Assert
        await act.Should().ThrowAsync<SylviaNG.Community.Application.Common.Exceptions.UnauthorizedException>();
    }

    [Fact]
    public async Task Publish_ShouldReturnOk()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SylviaNG.Community.Application.Features.Elections.Commands.ElectionPublish.ElectionPublishCommand>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Publish(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Close_ShouldReturnOk()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SylviaNG.Community.Application.Features.Elections.Commands.ElectionClose.ElectionCloseCommand>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Close(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnOk()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SylviaNG.Community.Application.Features.Elections.Commands.ElectionDelete.ElectionDeleteCommand>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task GetResults_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new ElectionResultsResponse { ElectionId = 1, TotalVotes = 3 };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SylviaNG.Community.Application.Features.Elections.Queries.ElectionGetResults.ElectionGetResultsQuery>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetResults(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }
}
