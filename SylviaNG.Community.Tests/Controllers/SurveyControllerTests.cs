using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyCreate;
using SylviaNG.Community.Application.Features.Surveys.Commands.SurveyResponseSubmit;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetAllPaged;
using SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetById;
using SylviaNG.Community.Application.Features.Surveys.Queries.SurveyResponseGetAllPaged;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class SurveyControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly SurveyController _controller;

    public SurveyControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new SurveyController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithResult()
    {
        var expected = new SurveyDetailResponse { SurveyId = 1, Title = "Engagement Pulse" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<SurveyGetByIdQuery>(), default)).ReturnsAsync(expected);

        var result = await _controller.GetById(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetPaged_ShouldReturnOkWithPagedResult()
    {
        var expected = new PagedResult<SurveyDetailResponse>
        {
            Data = new List<SurveyDetailResponse> { new() { SurveyId = 1, Title = "Engagement Pulse" } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<SurveyGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        var result = await _controller.GetPaged(new PagedRequest());

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithNewId()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<SurveyCreateCommand>(), default)).ReturnsAsync(42L);

        var result = await _controller.Create(new SurveyCreateRequest { Title = "Engagement Pulse", SurveyType = "Pulse" });

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(42L);
    }

    [Fact]
    public async Task SubmitResponse_ShouldReturnOkWithNewResponseId()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<SurveyResponseSubmitCommand>(), default)).ReturnsAsync(99L);

        var result = await _controller.SubmitResponse(1, new SurveySubmissionRequest { EmployeeId = 5 });

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(99L);
    }

    [Fact]
    public async Task GetResponses_ShouldReturnOkWithPagedResult()
    {
        var expected = new PagedResult<SurveySubmissionResponse>
        {
            Data = new List<SurveySubmissionResponse> { new() { ResponseId = 1, SurveyId = 1, EmployeeId = 5 } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<SurveyResponseGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        var result = await _controller.GetResponses(1, new PagedRequest());

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }
}
