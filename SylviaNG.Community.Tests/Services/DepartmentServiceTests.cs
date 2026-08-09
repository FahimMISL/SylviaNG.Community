using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Departments.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Tests.Services;

public class DepartmentServiceTests
{
    private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DepartmentService _service;

    public DepartmentServiceTests()
    {
        _departmentRepositoryMock = new Mock<IDepartmentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new DepartmentService(_departmentRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new DepartmentCreateRequest { Name = "Engineering" };

        _departmentRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(false);
        _departmentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Department>()))
            .Callback<Department>(d => d.DepartmentId = 1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        // Arrange
        var request = new DepartmentCreateRequest { Name = "Engineering" };
        _departmentRepositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>().WithMessage("*Engineering*");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _departmentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Department?)null);

        // Act
        var act = () => _service.GetByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
