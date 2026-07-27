using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.PostAttachments.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class PostAttachmentServiceTests
{
    private readonly Mock<IPostAttachmentRepository> _attachmentRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PostAttachmentService _service;

    public PostAttachmentServiceTests()
    {
        _attachmentRepositoryMock = new Mock<IPostAttachmentRepository>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new PostAttachmentService(_attachmentRepositoryMock.Object, _postRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { PostId = 1 });
        _attachmentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PostAttachment>()))
            .Callback<PostAttachment>(a => a.AttachmentId = 3);

        var request = new PostAttachmentAddRequest { FileName = "doc.pdf", FilePath = "/files/doc.pdf", FileSize = 1024 };

        // Act
        var result = await _service.AddAsync(1, request);

        // Assert
        result.Should().Be(3);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenPostNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        // Act
        var act = () => _service.AddAsync(1, new PostAttachmentAddRequest { FileName = "a.png", FilePath = "/a.png", FileSize = 10 });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
