using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;
using SylviaNG.Community.Application.Interfaces.Externals;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class EmployeeCredentialServiceTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IEmployeeKeycloakAccountRepository> _accountRepositoryMock;
    private readonly Mock<IKeycloakAdminClient> _keycloakAdminClientMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly EmployeeCredentialService _service;

    public EmployeeCredentialServiceTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _accountRepositoryMock = new Mock<IEmployeeKeycloakAccountRepository>();
        _keycloakAdminClientMock = new Mock<IKeycloakAdminClient>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Keycloak:DefaultRealmRole"] = "Employee" })
            .Build();

        _service = new EmployeeCredentialService(
            _employeeRepositoryMock.Object,
            _accountRepositoryMock.Object,
            _keycloakAdminClientMock.Object,
            _unitOfWorkMock.Object,
            configuration);
    }

    private static Employee ActiveEmployee(long id = 1) => new()
    {
        EmployeeId = id,
        EmployeeName = "Ayesha Rahman",
        Email = "ayesha.rahman@sylviang.example",
        IsActive = true
    };

    private EmployeeCredentialCreateRequest ValidRequest(long employeeId = 1) => new()
    {
        EmployeeId = employeeId,
        Username = "ayesha.rahman",
        TemporaryPassword = "Temp1234"
    };

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldCreateKeycloakUserAssignRoleAndPersistAccount()
    {
        // Arrange
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ActiveEmployee());
        _accountRepositoryMock.Setup(r => r.ExistsByEmployeeIdAsync(1)).ReturnsAsync(false);
        _keycloakAdminClientMock
            .Setup(k => k.CreateUserAsync("ayesha.rahman", "ayesha.rahman@sylviang.example", "Ayesha", "Rahman", "Temp1234", 1))
            .ReturnsAsync("kc-user-guid-1");

        // Act
        var result = await _service.CreateAsync(ValidRequest());

        // Assert
        result.EmployeeId.Should().Be(1);
        result.Username.Should().Be("ayesha.rahman");
        result.KeycloakUserId.Should().Be("kc-user-guid-1");
        result.AssignedRole.Should().Be("Employee");
        _keycloakAdminClientMock.Verify(k => k.AssignRealmRoleAsync("kc-user-guid-1", "Employee"), Times.Once);
        _accountRepositoryMock.Verify(r => r.AddAsync(It.Is<EmployeeKeycloakAccount>(a =>
            a.EmployeeId == 1 && a.KeycloakUserId == "kc-user-guid-1" && a.Username == "ayesha.rahman" && a.AssignedRole == "Employee")), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExplicitRole_ShouldAssignThatRoleInsteadOfDefault()
    {
        // Arrange
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ActiveEmployee());
        _accountRepositoryMock.Setup(r => r.ExistsByEmployeeIdAsync(1)).ReturnsAsync(false);
        _keycloakAdminClientMock
            .Setup(k => k.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
            .ReturnsAsync("kc-user-guid-1");

        var request = ValidRequest();
        request.Role = "HR";

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.AssignedRole.Should().Be("HR");
        _keycloakAdminClientMock.Verify(k => k.AssignRealmRoleAsync("kc-user-guid-1", "HR"), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEmployeeNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee?)null);

        // Act
        var act = () => _service.CreateAsync(ValidRequest());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _keycloakAdminClientMock.Verify(k => k.CreateUserAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenEmployeeDeactivated_ShouldThrowForbiddenException()
    {
        // Arrange
        var employee = ActiveEmployee();
        employee.IsActive = false;
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);

        // Act
        var act = () => _service.CreateAsync(ValidRequest());

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _keycloakAdminClientMock.Verify(k => k.CreateUserAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenEmployeeAlreadyHasAnAccount_ShouldThrowDuplicateException()
    {
        // Arrange
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ActiveEmployee());
        _accountRepositoryMock.Setup(r => r.ExistsByEmployeeIdAsync(1)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(ValidRequest());

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
        _keycloakAdminClientMock.Verify(k => k.CreateUserAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenKeycloakRejectsAsDuplicate_ShouldPropagateDuplicateExceptionAndNotPersist()
    {
        // Arrange
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ActiveEmployee());
        _accountRepositoryMock.Setup(r => r.ExistsByEmployeeIdAsync(1)).ReturnsAsync(false);
        _keycloakAdminClientMock
            .Setup(k => k.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
            .ThrowsAsync(new DuplicateException("EmployeeKeycloakAccount", "Username or email", "ayesha.rahman"));

        // Act
        var act = () => _service.CreateAsync(ValidRequest());

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeeKeycloakAccount>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenKeycloakDeniesAdminPermission_ShouldPropagateExternalServiceExceptionAndNotPersist()
    {
        // Arrange
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ActiveEmployee());
        _accountRepositoryMock.Setup(r => r.ExistsByEmployeeIdAsync(1)).ReturnsAsync(false);
        _keycloakAdminClientMock
            .Setup(k => k.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
            .ThrowsAsync(new ExternalServiceException("Keycloak denied permission to create the Keycloak user."));

        // Act
        var act = () => _service.CreateAsync(ValidRequest());

        // Assert
        await act.Should().ThrowAsync<ExternalServiceException>();
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeeKeycloakAccount>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenAccountExists_ShouldCallKeycloakWithItsUserId()
    {
        // Arrange
        _accountRepositoryMock.Setup(r => r.GetByEmployeeIdAsync(1))
            .ReturnsAsync(new EmployeeKeycloakAccount { EmployeeId = 1, KeycloakUserId = "kc-user-guid-1", Username = "ayesha.rahman", AssignedRole = "Employee" });

        // Act
        await _service.ResetPasswordAsync(1, "NewTemp1234");

        // Assert
        _keycloakAdminClientMock.Verify(k => k.ResetPasswordAsync("kc-user-guid-1", "NewTemp1234"), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenEmployeeHasNoAccount_ShouldThrowNotFoundExceptionAndNotCallKeycloak()
    {
        // Arrange
        _accountRepositoryMock.Setup(r => r.GetByEmployeeIdAsync(1)).ReturnsAsync((EmployeeKeycloakAccount?)null);

        // Act
        var act = () => _service.ResetPasswordAsync(1, "NewTemp1234");

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _keycloakAdminClientMock.Verify(k => k.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
