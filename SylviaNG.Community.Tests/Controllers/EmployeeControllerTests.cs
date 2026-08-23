using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeCreate;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeDeactivate;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateCoverPhoto;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdatePhoto;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateProfile;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetAllForManagement;
using SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetAllPaged;
using SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetById;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Controllers;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Controllers;

public class EmployeeControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly EmployeeController _controller;

    public EmployeeControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _controller = new EmployeeController(_mediatorMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task GetDirectoryPaged_ShouldReturnOkWithPagedResult()
    {
        // Arrange
        var expected = new PagedResult<EmployeeDirectoryCardResponse>
        {
            Data = new List<EmployeeDirectoryCardResponse> { new() { EmployeeId = 1, EmployeeName = "Ayesha Rahman" } },
            TotalCount = 1
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeGetAllPagedQuery>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetDirectoryPaged(new EmployeeFilterRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetById_ShouldPassCurrentUserContextToQuery()
    {
        // Arrange
        _currentUserMock.SetupGet(c => c.EmployeeId).Returns(2);
        _currentUserMock.SetupGet(c => c.IsHrOrAdmin).Returns(true);

        var expected = new EmployeeResponse { EmployeeId = 1, EmployeeName = "Ayesha Rahman" };
        EmployeeGetByIdQuery? capturedQuery = null;

        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeGetByIdQuery>(), default))
            .Callback<IRequest<EmployeeResponse>, CancellationToken>((q, _) => capturedQuery = (EmployeeGetByIdQuery)q)
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
        capturedQuery.Should().NotBeNull();
        capturedQuery!.EmployeeId.Should().Be(1);
        capturedQuery.ViewerEmployeeId.Should().Be(2);
        capturedQuery.ViewerIsHrAdmin.Should().BeTrue();
    }

    [Fact]
    public async Task Create_WithValidRequest_ShouldReturnCreatedId()
    {
        // Arrange
        var request = new EmployeeCreateRequest
        {
            EmployeeName = "New Hire",
            Email = "new.hire@sylviang.example",
            DesignationId = 1,
            DepartmentId = 1,
            SiteId = 1
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeCreateCommand>(), default))
            .ReturnsAsync(1L);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(1L);
    }

    [Fact]
    public async Task UpdateProfile_ShouldPassCurrentUserAsViewer()
    {
        // Arrange
        _currentUserMock.SetupGet(c => c.EmployeeId).Returns(1);

        var request = new EmployeeUpdateProfileRequest
        {
            Bio = "Updated bio",
            Phone = "+880-1710-000001",
            Email = "ayesha.rahman@sylviang.example",
            Extension = "1001",
            ContactLinks = new List<EmployeeContactLinkItem>
            {
                new() { Id = null, Platform = "LinkedIn", Url = "https://linkedin.com/in/ayesha", Visibility = SylviaNG.Community.Domain.Enums.ContactVisibilityEnum.Public }
            }
        };
        EmployeeUpdateProfileCommand? capturedCommand = null;

        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeUpdateProfileCommand>(), default))
            .Callback<IRequest<Unit>, CancellationToken>((c, _) => capturedCommand = (EmployeeUpdateProfileCommand)c)
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.UpdateProfile(1, request);

        // Assert
        result.Should().BeOfType<OkResult>();
        capturedCommand.Should().NotBeNull();
        capturedCommand!.EmployeeId.Should().Be(1);
        capturedCommand.ViewerEmployeeId.Should().Be(1);
        capturedCommand.Request.Phone.Should().Be("+880-1710-000001");
        capturedCommand.Request.ContactLinks.Should().ContainSingle(l => l.Platform == "LinkedIn");
    }

    [Fact]
    public async Task UpdatePhoto_ShouldPassCurrentUserAsViewer()
    {
        // Arrange
        _currentUserMock.SetupGet(c => c.EmployeeId).Returns(1);

        var request = new EmployeeUpdatePhotoRequest { StoragePath = "uploads/employee-photo/2026-07/guid.jpg" };
        EmployeeUpdatePhotoCommand? capturedCommand = null;

        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeUpdatePhotoCommand>(), default))
            .Callback<IRequest<Unit>, CancellationToken>((c, _) => capturedCommand = (EmployeeUpdatePhotoCommand)c)
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.UpdatePhoto(1, request);

        // Assert
        result.Should().BeOfType<OkResult>();
        capturedCommand.Should().NotBeNull();
        capturedCommand!.EmployeeId.Should().Be(1);
        capturedCommand.ViewerEmployeeId.Should().Be(1);
        capturedCommand.Request.StoragePath.Should().Be(request.StoragePath);
    }

    [Fact]
    public async Task UpdateCoverPhoto_ShouldPassCurrentUserAsViewer()
    {
        // Arrange
        _currentUserMock.SetupGet(c => c.EmployeeId).Returns(1);

        var request = new EmployeeUpdateCoverPhotoRequest { StoragePath = "uploads/employee-cover/2026-07/guid.jpg" };
        EmployeeUpdateCoverPhotoCommand? capturedCommand = null;

        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeUpdateCoverPhotoCommand>(), default))
            .Callback<IRequest<Unit>, CancellationToken>((c, _) => capturedCommand = (EmployeeUpdateCoverPhotoCommand)c)
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.UpdateCoverPhoto(1, request);

        // Assert
        result.Should().BeOfType<OkResult>();
        capturedCommand.Should().NotBeNull();
        capturedCommand!.EmployeeId.Should().Be(1);
        capturedCommand.ViewerEmployeeId.Should().Be(1);
        capturedCommand.Request.StoragePath.Should().Be(request.StoragePath);
    }

    [Fact]
    public async Task Deactivate_WithValidId_ShouldReturnOk()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeDeactivateCommand>(), default))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.Deactivate(1);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task GetManagementPaged_ShouldReturnOkWithPagedResult()
    {
        // Arrange
        var expected = new PagedResult<EmployeeManagementRowResponse>
        {
            Data = new List<EmployeeManagementRowResponse> { new() { EmployeeId = 1, IsActive = false } },
            TotalCount = 1
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeGetAllForManagementQuery>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetManagementPaged(new EmployeeFilterRequest());

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }
}
