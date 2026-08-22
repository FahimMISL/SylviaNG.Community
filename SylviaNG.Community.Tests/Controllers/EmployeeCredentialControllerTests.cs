using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialCreate;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialResetPassword;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;
using SylviaNG.Community.Controllers;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Controllers;

public class EmployeeCredentialControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly EmployeeCredentialController _controller;

    public EmployeeCredentialControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new EmployeeCredentialController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithResult()
    {
        // Arrange
        var expected = new EmployeeCredentialResponse
        {
            EmployeeId = 1,
            Username = "ayesha.rahman",
            KeycloakUserId = "kc-user-guid-1",
            AssignedRole = "Employee"
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeCredentialCreateCommand>(), default)).ReturnsAsync(expected);

        // Act
        var result = await _controller.Create(1, new EmployeeCredentialCreateRequest { Username = "ayesha.rahman", TemporaryPassword = "Temp1234" });

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_ShouldOverrideRequestEmployeeIdWithRouteValue()
    {
        // Arrange
        EmployeeCredentialCreateCommand? capturedCommand = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeCredentialCreateCommand>(), default))
            .Callback<IRequest<EmployeeCredentialResponse>, CancellationToken>((cmd, _) => capturedCommand = (EmployeeCredentialCreateCommand)cmd)
            .ReturnsAsync(new EmployeeCredentialResponse());

        // Act - route employeeId (7) should win even if the body carries a different value
        await _controller.Create(7, new EmployeeCredentialCreateRequest { EmployeeId = 999, Username = "x", TemporaryPassword = "Temp1234" });

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.Request.EmployeeId.Should().Be(7);
    }

    [Fact]
    public async Task ResetPassword_ShouldSendCommandWithRouteEmployeeIdAndReturnOk()
    {
        // Arrange
        EmployeeCredentialResetPasswordCommand? capturedCommand = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<EmployeeCredentialResetPasswordCommand>(), default))
            .Callback<IRequest, CancellationToken>((cmd, _) => capturedCommand = (EmployeeCredentialResetPasswordCommand)cmd)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ResetPassword(7, new EmployeeCredentialResetPasswordRequest { TemporaryPassword = "NewTemp1234" });

        // Assert
        result.Should().BeOfType<OkResult>();
        capturedCommand.Should().NotBeNull();
        capturedCommand!.EmployeeId.Should().Be(7);
        capturedCommand.Request.TemporaryPassword.Should().Be("NewTemp1234");
    }
}
