using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Skills.Commands.SkillCreate;
using SylviaNG.Community.Application.Features.Skills.Models;
using SylviaNG.Community.Application.Features.Skills.Queries.SkillGetAllPaged;
using SylviaNG.Community.Application.Features.Skills.Queries.SkillGetById;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class SkillControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly SkillController _controller;

    public SkillControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new SkillController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new SkillResponse { SkillId = 1, Name = "C#" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<SkillGetByIdQuery>(), default)).ReturnsAsync(expected);

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
        var expected = new PagedResult<SkillResponse>
        {
            Data = new List<SkillResponse> { new() { SkillId = 1, Name = "C#" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<SkillGetAllPagedQuery>(), default)).ReturnsAsync(expected);

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
        _mediatorMock.Setup(m => m.Send(It.IsAny<SkillCreateCommand>(), default)).ReturnsAsync(42L);

        // Act
        var result = await _controller.Create(new SkillCreateRequest { Name = "C#" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }
}
