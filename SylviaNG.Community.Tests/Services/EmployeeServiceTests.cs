using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Interfaces.Externals;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Tests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICoreGrpcClient> _coreGrpcClientMock;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _coreGrpcClientMock = new Mock<ICoreGrpcClient>();
        _service = new EmployeeService(_repositoryMock.Object, _unitOfWorkMock.Object, _coreGrpcClientMock.Object, Mock.Of<ILogger<EmployeeService>>());
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithValidRequest_ShouldReturnIdAndGenerateEmployeeCode()
    {
        // Arrange
        var request = new EmployeeCreateRequest
        {
            EmployeeName = "Ayesha Rahman",
            Email = "ayesha.rahman@sylviang.example",
            DesignationId = 1,
            DepartmentId = 1,
            SiteId = 1
        };

        _repositoryMock.Setup(r => r.ExistsByEmailAsync(request.Email, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
            .Callback<Employee>(e => e.EmployeeId = 1);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(1);
        _repositoryMock.Verify(r => r.Update(It.Is<Employee>(e => e.EmployeeCode == "EMP00001")), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithDuplicateEmail_ShouldThrowDuplicateException()
    {
        // Arrange
        var request = new EmployeeCreateRequest { EmployeeName = "Existing", Email = "existing@sylviang.example" };
        _repositoryMock.Setup(r => r.ExistsByEmailAsync(request.Email, null)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>().WithMessage("*existing@sylviang.example*");
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateProfileAsync_WhenViewerIsNotOwner_ShouldThrowForbiddenException()
    {
        // Arrange
        var request = new EmployeeUpdateProfileRequest { Bio = "New bio" };

        // Act
        var act = () => _service.UpdateProfileAsync(employeeId: 1, request, viewerEmployeeId: 2);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateProfileAsync_WhenViewerIsOwner_ShouldApplyUpdateAndSave()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, Bio = "Old bio" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var request = new EmployeeUpdateProfileRequest { Bio = "New bio", Skills = "C#, Angular" };

        // Act
        await _service.UpdateProfileAsync(employeeId: 1, request, viewerEmployeeId: 1);

        // Assert
        entity.Bio.Should().Be("New bio");
        entity.Skills.Should().Be("C#, Angular");
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateProfileAsync_WhenViewerIsOwner_ShouldApplyAchievementsAndCommunityContributions()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, Achievements = "Old achievement" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var request = new EmployeeUpdateProfileRequest { Achievements = "Employee of the month", CommunityContributions = "Mentored 3 interns" };

        // Act
        await _service.UpdateProfileAsync(employeeId: 1, request, viewerEmployeeId: 1);

        // Assert
        entity.Achievements.Should().Be("Employee of the month");
        entity.CommunityContributions.Should().Be("Mentored 3 interns");
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdatePhotoAsync_WhenViewerIsNotOwner_ShouldThrowForbiddenException()
    {
        // Act
        var act = () => _service.UpdatePhotoAsync(employeeId: 1, storagePath: "uploads/employee-photo/2026-07/guid.jpg", viewerEmployeeId: 2);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdatePhotoAsync_WhenViewerIsOwner_ShouldSetPhotoUrlAndSave()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, PhotoUrl = null };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.UpdatePhotoAsync(employeeId: 1, storagePath: "uploads/employee-photo/2026-07/guid.jpg", viewerEmployeeId: 1);

        // Assert
        entity.PhotoUrl.Should().Be("uploads/employee-photo/2026-07/guid.jpg");
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateCoverPhotoAsync_WhenViewerIsNotOwner_ShouldThrowForbiddenException()
    {
        // Act
        var act = () => _service.UpdateCoverPhotoAsync(employeeId: 1, storagePath: "uploads/employee-cover/2026-07/guid.jpg", viewerEmployeeId: 2);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateCoverPhotoAsync_WhenViewerIsOwner_ShouldSetCoverPhotoUrlAndSave()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, CoverPhotoUrl = null };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.UpdateCoverPhotoAsync(employeeId: 1, storagePath: "uploads/employee-cover/2026-07/guid.jpg", viewerEmployeeId: 1);

        // Assert
        entity.CoverPhotoUrl.Should().Be("uploads/employee-cover/2026-07/guid.jpg");
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeactivateAsync_WithExistingId_ShouldSetIsActiveFalse()
    {
        // Arrange
        var entity = new Employee { EmployeeId = 1, IsActive = true };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.DeactivateAsync(1);

        // Assert
        entity.IsActive.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeactivateAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

        // Act
        var act = () => _service.DeactivateAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ForNonOwnerNonHrViewer_ShouldHidePrivateContactFields()
    {
        // Arrange
        var entity = new Employee
        {
            EmployeeId = 1,
            EmployeeName = "Ayesha Rahman",
            Phone = "+880-1710-000001",
            Email = "ayesha.rahman@sylviang.example",
            PhoneVisibility = ContactVisibilityEnum.Private,
            EmailVisibility = ContactVisibilityEnum.Public,
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act - viewer is a different employee (2), not HR/Admin
        var result = await _service.GetByIdAsync(employeeId: 1, viewerEmployeeId: 2, viewerIsHrAdmin: false);

        // Assert
        result.Phone.Should().BeNull("phone is Private and the viewer is neither the owner nor HR/Admin");
        result.Email.Should().Be("ayesha.rahman@sylviang.example", "email is Public");
        result.IsOwnProfile.Should().BeFalse();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ForOwner_ShouldAlwaysShowPrivateContactFields()
    {
        // Arrange
        var entity = new Employee
        {
            EmployeeId = 1,
            Phone = "+880-1710-000001",
            PhoneVisibility = ContactVisibilityEnum.Private,
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdAsync(employeeId: 1, viewerEmployeeId: 1, viewerIsHrAdmin: false);

        // Assert
        result.Phone.Should().Be("+880-1710-000001");
        result.IsOwnProfile.Should().BeTrue();
    }
}
