using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.EmployeeSkills.Models;
using SylviaNG.Community.Application.Features.Skills.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class SkillServiceTests
{
    private readonly Mock<ISkillRepository> _skillRepositoryMock;
    private readonly Mock<IEmployeeSkillRepository> _employeeSkillRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SkillService _service;

    public SkillServiceTests()
    {
        _skillRepositoryMock = new Mock<ISkillRepository>();
        _employeeSkillRepositoryMock = new Mock<IEmployeeSkillRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new SkillService(_skillRepositoryMock.Object, _employeeSkillRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new SkillCreateRequest { Name = "C#" };

        _skillRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(false);
        _skillRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Skill>()))
            .Callback<Skill>(s => s.SkillId = 1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        // Arrange
        var request = new SkillCreateRequest { Name = "C#" };
        _skillRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>().WithMessage("*C#*");
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _skillRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Skill?)null);

        // Act
        var act = () => _service.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AssignToEmployeeAsync_WhenAlreadyAssigned_ShouldThrowDuplicateException()
    {
        // Arrange
        _skillRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Skill { SkillId = 1, Name = "C#" });
        _employeeSkillRepositoryMock.Setup(r => r.ExistsAsync(5, 1)).ReturnsAsync(true);

        // Act
        var act = () => _service.AssignToEmployeeAsync(5, new EmployeeSkillAssignRequest { SkillId = 1 });

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task AssignToEmployeeAsync_WhenSkillNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _skillRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Skill?)null);

        // Act
        var act = () => _service.AssignToEmployeeAsync(5, new EmployeeSkillAssignRequest { SkillId = 1 });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RemoveFromEmployeeAsync_WhenAssignmentExists_ShouldDeleteAndSave()
    {
        // Arrange
        var assignment = new EmployeeSkill { EmployeeSkillId = 10, EmployeeId = 5, SkillId = 1 };
        _employeeSkillRepositoryMock.Setup(r => r.GetAsync(5, 1)).ReturnsAsync(assignment);

        // Act
        await _service.RemoveFromEmployeeAsync(5, 1);

        // Assert
        _employeeSkillRepositoryMock.Verify(r => r.Delete(assignment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveFromEmployeeAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _employeeSkillRepositoryMock.Setup(r => r.GetAsync(5, 1)).ReturnsAsync((EmployeeSkill?)null);

        // Act
        var act = () => _service.RemoveFromEmployeeAsync(5, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
