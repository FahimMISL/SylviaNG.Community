using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Auth.Models;
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
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _credentialRepositoryMock = new Mock<ICredentialRepository>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _service = new AuthService(_credentialRepositoryMock.Object, _jwtTokenGeneratorMock.Object);
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
