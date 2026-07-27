using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.ContentReports.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class ContentReportServiceTests
{
    private readonly Mock<IContentReportRepository> _contentReportRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ContentReportService _service;

    public ContentReportServiceTests()
    {
        _contentReportRepositoryMock = new Mock<IContentReportRepository>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new ContentReportService(_contentReportRepositoryMock.Object, _postRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1 });
        _contentReportRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ContentReport>()))
            .Callback<ContentReport>(c => c.ReportId = 8);

        var request = new ContentReportCreateRequest { ReportedBy = 2, PostId = 1, Reason = "Spam" };

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(8);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenPostNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        // Act
        var act = () => _service.CreateAsync(new ContentReportCreateRequest { ReportedBy = 2, PostId = 1, Reason = "Spam" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ResolveAsync_WithValidRequest_ShouldUpdateStatusAndSave()
    {
        // Arrange
        var report = new ContentReport { ReportId = 1, Status = "Pending" };
        _contentReportRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(report);

        // Act
        await _service.ResolveAsync(1, new ContentReportResolveRequest { ReviewedBy = 9, Status = "Resolved" });

        // Assert
        report.Status.Should().Be("Resolved");
        report.ReviewedBy.Should().Be(9);
        report.ReviewedAt.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ResolveAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _contentReportRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ContentReport?)null);

        // Act
        var act = () => _service.ResolveAsync(1, new ContentReportResolveRequest { ReviewedBy = 9, Status = "Resolved" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
