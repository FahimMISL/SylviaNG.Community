using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Auth.Models;
using SylviaNG.Community.Application.Interfaces.Externals;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<ICredentialRepository> _credentialRepositoryMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<IKeycloakAdminClient> _keycloakAdminClientMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _credentialRepositoryMock = new Mock<ICredentialRepository>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _keycloakAdminClientMock = new Mock<IKeycloakAdminClient>();
        _service = new AuthService(_credentialRepositoryMock.Object, _jwtTokenGeneratorMock.Object, _keycloakAdminClientMock.Object);
    }

    private static Credential MakeCredential(string username, string plainTextPassword) => new()
    {
        Id = 1,
        Username = username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainTextPassword, workFactor: 11),
        DisplayName = "Test User",
        Role = "Employee",
        EmployeeId = 1,
        IsActive = true,
    };

    [Fact]
    public async Task LoginAsync_WithValidLocalCredential_ShouldReturnLocalJwtWithoutCallingKeycloak()
    {
        // Arrange
        var credential = MakeCredential("admin", "Admin@123");
        _credentialRepositoryMock.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(credential);
        _jwtTokenGeneratorMock.Setup(j => j.GenerateToken(credential)).Returns(("local-jwt", DateTime.UtcNow.AddHours(1)));

        // Act
        var result = await _service.LoginAsync(new LoginRequestDto { Username = "admin", Password = "Admin@123" });

        // Assert
        result.AccessToken.Should().Be("local-jwt");
        result.DisplayName.Should().Be("Test User");
        _keycloakAdminClientMock.Verify(k => k.TryPasswordLoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithWrongLocalPasswordButValidKeycloakCredential_ShouldReturnKeycloakBackedToken()
    {
        // Arrange - "farhana.akter" exists locally (demo account) but the caller is using her
        // real Keycloak password (e.g. set via Grant Access/Reset Password), not the local one.
        var credential = MakeCredential("farhana.akter", "HR@123");
        _credentialRepositoryMock.Setup(r => r.GetByUsernameAsync("farhana.akter")).ReturnsAsync(credential);

        var keycloakResult = new KeycloakLoginResult
        {
            AccessToken = "keycloak-jwt",
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5),
            DisplayName = "Farhana Akter",
            Role = "HR",
            EmployeeId = 3,
        };
        _keycloakAdminClientMock.Setup(k => k.TryPasswordLoginAsync("farhana.akter", "NewTemp1234")).ReturnsAsync(keycloakResult);

        // Act
        var result = await _service.LoginAsync(new LoginRequestDto { Username = "farhana.akter", Password = "NewTemp1234" });

        // Assert
        result.AccessToken.Should().Be("keycloak-jwt");
        result.DisplayName.Should().Be("Farhana Akter");
        result.Role.Should().Be("HR");
        result.EmployeeId.Should().Be(3);
        _jwtTokenGeneratorMock.Verify(j => j.GenerateToken(It.IsAny<Credential>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithUnknownLocalUsernameButValidKeycloakCredential_ShouldReturnKeycloakBackedToken()
    {
        // Arrange
        _credentialRepositoryMock.Setup(r => r.GetByUsernameAsync("nabil.khan")).ReturnsAsync((Credential?)null);

        var keycloakResult = new KeycloakLoginResult
        {
            AccessToken = "keycloak-jwt",
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5),
            DisplayName = null,
            Role = null,
            EmployeeId = 5,
        };
        _keycloakAdminClientMock.Setup(k => k.TryPasswordLoginAsync("nabil.khan", "Temp1234")).ReturnsAsync(keycloakResult);

        // Act
        var result = await _service.LoginAsync(new LoginRequestDto { Username = "nabil.khan", Password = "Temp1234" });

        // Assert - falls back to the submitted username / a default "Employee" role when Keycloak's
        // token didn't carry a name/role claim.
        result.AccessToken.Should().Be("keycloak-jwt");
        result.Username.Should().Be("nabil.khan");
        result.DisplayName.Should().Be("nabil.khan");
        result.Role.Should().Be("Employee");
        result.EmployeeId.Should().Be(5);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentialsEverywhere_ShouldThrowUnauthorizedException()
    {
        // Arrange
        _credentialRepositoryMock.Setup(r => r.GetByUsernameAsync("ghost")).ReturnsAsync((Credential?)null);
        _keycloakAdminClientMock.Setup(k => k.TryPasswordLoginAsync("ghost", "wrong")).ReturnsAsync((KeycloakLoginResult?)null);

        // Act
        var act = () => _service.LoginAsync(new LoginRequestDto { Username = "ghost", Password = "wrong" });

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Invalid username or password.");
    }

    [Fact]
    public async Task ChangePasswordAsync_WithCorrectCurrentPassword_ShouldUpdateHash()
    {
        // Arrange
        var credential = MakeCredential("ayesha.rahman", "OldPass@123");
        _credentialRepositoryMock.Setup(r => r.GetByUsernameAsync("ayesha.rahman")).ReturnsAsync(credential);

        var request = new ChangePasswordRequestDto
        {
            CurrentPassword = "OldPass@123",
            NewPassword = "NewPass@456",
            ConfirmNewPassword = "NewPass@456",
        };

        // Act
        await _service.ChangePasswordAsync("ayesha.rahman", request);

        // Assert
        _credentialRepositoryMock.Verify(
            r => r.UpdatePasswordHashAsync("ayesha.rahman", It.Is<string>(hash => BCrypt.Net.BCrypt.Verify("NewPass@456", hash))),
            Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithWrongCurrentPassword_ShouldThrowUnauthorized()
    {
        // Arrange
        var credential = MakeCredential("ayesha.rahman", "OldPass@123");
        _credentialRepositoryMock.Setup(r => r.GetByUsernameAsync("ayesha.rahman")).ReturnsAsync(credential);

        var request = new ChangePasswordRequestDto
        {
            CurrentPassword = "WrongPassword",
            NewPassword = "NewPass@456",
            ConfirmNewPassword = "NewPass@456",
        };

        // Act
        var act = () => _service.ChangePasswordAsync("ayesha.rahman", request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
        _credentialRepositoryMock.Verify(r => r.UpdatePasswordHashAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithUnknownUsername_ShouldThrowUnauthorized()
    {
        // Arrange
        _credentialRepositoryMock.Setup(r => r.GetByUsernameAsync("ghost")).ReturnsAsync((Credential?)null);

        var request = new ChangePasswordRequestDto
        {
            CurrentPassword = "Whatever@123",
            NewPassword = "NewPass@456",
            ConfirmNewPassword = "NewPass@456",
        };

        // Act
        var act = () => _service.ChangePasswordAsync("ghost", request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
