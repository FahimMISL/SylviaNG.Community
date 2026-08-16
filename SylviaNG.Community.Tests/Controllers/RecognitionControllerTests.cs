using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Queries.RecognitionGetAllPaged;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class RecognitionControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly RecognitionController _controller;

    public RecognitionControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _controller = new RecognitionController(_mediatorMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task GetPaged_ShouldPassCurrentUserAsViewerContext()
    {
        // Arrange
        _currentUserMock.SetupGet(c => c.EmployeeId).Returns(5);
        _currentUserMock.SetupGet(c => c.IsHrOrAdmin).Returns(false);

        var expected = new PagedResult<RecognitionResponse> { Data = new List<RecognitionResponse>(), TotalCount = 0 };
        RecognitionGetAllPagedQuery? capturedQuery = null;

        _mediatorMock.Setup(m => m.Send(It.IsAny<RecognitionGetAllPagedQuery>(), default))
            .Callback<IRequest<PagedResult<RecognitionResponse>>, CancellationToken>((q, _) => capturedQuery = (RecognitionGetAllPagedQuery)q)
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(new PagedRequest(), senderId: null, recipientId: 5);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
        capturedQuery.Should().NotBeNull();
        capturedQuery!.RecipientId.Should().Be(5);
        capturedQuery.ViewerEmployeeId.Should().Be(5);
        capturedQuery.ViewerIsHrAdmin.Should().BeFalse();
    }
}
