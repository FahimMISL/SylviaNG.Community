using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class RecognitionServiceTests
{
    private readonly Mock<IRecognitionRepository> _recognitionRepositoryMock;
    private readonly Mock<IRecognitionReactionRepository> _recognitionReactionRepositoryMock;
    private readonly Mock<IRecognitionCommentRepository> _recognitionCommentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RecognitionService _service;

    public RecognitionServiceTests()
    {
        _recognitionRepositoryMock = new Mock<IRecognitionRepository>();
        _recognitionReactionRepositoryMock = new Mock<IRecognitionReactionRepository>();
        _recognitionCommentRepositoryMock = new Mock<IRecognitionCommentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new RecognitionService(
            _recognitionRepositoryMock.Object,
            _recognitionReactionRepositoryMock.Object,
            _recognitionCommentRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        var request = new RecognitionCreateRequest
        {
            SenderId = 1,
            RecipientId = 2,
            RecognitionType = "Peer",
            Message = "Great job!"
        };

        _recognitionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Recognition>()))
            .Callback<Recognition>(r => r.RecognitionId = 1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _recognitionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Recognition?)null);

        // Act
        var act = () => _service.GetByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddReactionAsync_WhenRecognitionNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _recognitionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Recognition?)null);

        // Act
        var act = () => _service.AddReactionAsync(1, new RecognitionReactionAddRequest { EmployeeId = 5, ReactionType = "Like" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddReactionAsync_WhenEmployeeAlreadyReacted_ShouldUpdateExistingReaction()
    {
        // Arrange
        _recognitionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Recognition { RecognitionId = 1 });
        var existing = new RecognitionReaction { ReactionId = 7, RecognitionId = 1, EmployeeId = 5, ReactionType = "Like" };
        _recognitionReactionRepositoryMock.Setup(r => r.GetAsync(1, 5)).ReturnsAsync(existing);

        // Act
        var result = await _service.AddReactionAsync(1, new RecognitionReactionAddRequest { EmployeeId = 5, ReactionType = "Clap" });

        // Assert
        result.Should().Be(7);
        existing.ReactionType.Should().Be("Clap");
        _recognitionReactionRepositoryMock.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public async Task RemoveReactionAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _recognitionReactionRepositoryMock.Setup(r => r.GetAsync(1, 5)).ReturnsAsync((RecognitionReaction?)null);

        // Act
        var act = () => _service.RemoveReactionAsync(1, 5);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
