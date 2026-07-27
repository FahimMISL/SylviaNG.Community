using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.AuditLogs.Models;
using SylviaNG.Community.Application.Features.AuditLogs.Queries.AuditLogGetAllPaged;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class AuditLogControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AuditLogController _controller;

    public AuditLogControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AuditLogController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetPaged_ShouldReturnOkWithPagedResult()
    {
        // Arrange
        var expected = new PagedResult<AuditLogResponse>
        {
            Data = new List<AuditLogResponse> { new() { AuditId = 1, TableName = "Teams", RecordId = 1, Action = "Update", PerformedBy = 2 } },
            TotalCount = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<AuditLogGetAllPagedQuery>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetPaged(new PagedRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }
}
