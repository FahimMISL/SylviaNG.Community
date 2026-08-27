using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Middlewares;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Middlewares;

public class EmployeeIdentityEnrichmentMiddlewareTests
{
    private readonly Mock<IEmployeeKeycloakAccountRepository> _repositoryMock = new();

    private static DefaultHttpContext CreateContext(ClaimsPrincipal? user = null)
    {
        var context = new DefaultHttpContext();
        if (user != null)
        {
            context.User = user;
        }
        return context;
    }

    private static ClaimsPrincipal CreateAuthenticatedUser(string? nameIdentifier, string? employeeIdClaim = null)
    {
        var claims = new List<Claim>();
        if (nameIdentifier != null) claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
        if (employeeIdClaim != null) claims.Add(new Claim("employee_id", employeeIdClaim));

        var identity = new ClaimsIdentity(claims, authenticationType: "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task InvokeAsync_WithMatchingKeycloakAccount_ShouldAddEmployeeIdClaim()
    {
        var user = CreateAuthenticatedUser(nameIdentifier: "kc-user-123");
        var context = CreateContext(user);
        _repositoryMock.Setup(r => r.GetByKeycloakUserIdAsync("kc-user-123"))
            .ReturnsAsync(new EmployeeKeycloakAccount { EmployeeId = 42, KeycloakUserId = "kc-user-123", Username = "jane.doe", AssignedRole = "Employee", IsActive = true });

        var nextCalled = false;
        var middleware = new EmployeeIdentityEnrichmentMiddleware(ctx => { nextCalled = true; return Task.CompletedTask; });

        await middleware.InvokeAsync(context, _repositoryMock.Object);

        context.User.FindFirst("employee_id")?.Value.Should().Be("42");
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WithInactiveKeycloakAccount_ShouldNotAddEmployeeIdClaim()
    {
        var user = CreateAuthenticatedUser(nameIdentifier: "kc-user-123");
        var context = CreateContext(user);
        _repositoryMock.Setup(r => r.GetByKeycloakUserIdAsync("kc-user-123"))
            .ReturnsAsync(new EmployeeKeycloakAccount { EmployeeId = 42, KeycloakUserId = "kc-user-123", Username = "jane.doe", AssignedRole = "Employee", IsActive = false });

        var middleware = new EmployeeIdentityEnrichmentMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context, _repositoryMock.Object);

        context.User.FindFirst("employee_id").Should().BeNull();
    }

    [Fact]
    public async Task InvokeAsync_WithNoMatchingAccount_ShouldNotAddEmployeeIdClaim()
    {
        var user = CreateAuthenticatedUser(nameIdentifier: "admin");
        var context = CreateContext(user);
        _repositoryMock.Setup(r => r.GetByKeycloakUserIdAsync("admin")).ReturnsAsync((EmployeeKeycloakAccount?)null);

        var middleware = new EmployeeIdentityEnrichmentMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context, _repositoryMock.Object);

        context.User.FindFirst("employee_id").Should().BeNull();
    }

    [Fact]
    public async Task InvokeAsync_WhenEmployeeIdClaimAlreadyPresent_ShouldNotCallRepository()
    {
        var user = CreateAuthenticatedUser(nameIdentifier: "kc-user-123", employeeIdClaim: "7");
        var context = CreateContext(user);

        var middleware = new EmployeeIdentityEnrichmentMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context, _repositoryMock.Object);

        _repositoryMock.Verify(r => r.GetByKeycloakUserIdAsync(It.IsAny<string>()), Times.Never);
        context.User.FindFirst("employee_id")?.Value.Should().Be("7");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthenticated_ShouldNotCallRepository()
    {
        var context = CreateContext();

        var middleware = new EmployeeIdentityEnrichmentMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context, _repositoryMock.Object);

        _repositoryMock.Verify(r => r.GetByKeycloakUserIdAsync(It.IsAny<string>()), Times.Never);
    }
}
