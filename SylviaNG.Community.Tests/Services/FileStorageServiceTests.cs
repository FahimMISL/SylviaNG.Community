using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class FileStorageServiceTests
{
    private readonly Mock<IFileStorageRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly FileStorageService _service;

    public FileStorageServiceTests()
    {
        _repositoryMock = new Mock<IFileStorageRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new FileStorageService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new FileStorageCreateRequest
        {
            Module = "Team",
            FileName = "abc123.png",
            OriginalFileName = "photo.png",
            StoragePath = "/uploads/abc123.png",
            FileSize = 1024,
            UploadedBy = 1
        };
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<FileStorage>()))
            .Callback<FileStorage>(f => f.FileId = 1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((FileStorage?)null);

        // Act
        var act = () => _service.GetByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPaginatedAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var request = new PagedRequest();
        var pagedResult = new PagedResult<FileStorage>
        {
            Data = new List<FileStorage> { new() { FileId = 1, Module = "Team", FileName = "a.png", OriginalFileName = "a.png", StoragePath = "/x", UploadedBy = 1 } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _repositoryMock.Setup(r => r.GetPaginatedAsync(request, null, null)).ReturnsAsync(pagedResult);

        // Act
        var result = await _service.GetPaginatedAsync(request, null, null);

        // Assert
        result.TotalCount.Should().Be(1);
        result.Data.Should().ContainSingle(f => f.FileId == 1);
    }
}
