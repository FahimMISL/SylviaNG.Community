using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Tests.Services;

public class MentionServiceTests
{
    private readonly Mock<IMentionRepository> _mentionRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MentionService _service;

    public MentionServiceTests()
    {
        _mentionRepositoryMock = new Mock<IMentionRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new MentionService(_mentionRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnId()
    {
        // Arrange
        _mentionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Mention>()))
            .Callback<Mention>(m => m.MentionId = 4);

        var request = new MentionCreateRequest { MentionedEmployeeId = 1, MentionedBy = 2, EntityType = "Post", EntityId = 10 };

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().Be(4);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPaginatedForEmployeeAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var pagedResult = new PagedResult<Mention>
        {
            Data = new List<Mention> { new() { MentionId = 1, MentionedEmployeeId = 1 } },
            TotalCount = 1
        };
        _mentionRepositoryMock.Setup(r => r.GetPaginatedForEmployeeAsync(1, It.IsAny<PagedRequest>())).ReturnsAsync(pagedResult);

        // Act
        var result = await _service.GetPaginatedForEmployeeAsync(1, new PagedRequest());

        // Assert
        result.Data.Should().ContainSingle(m => m.MentionId == 1);
    }
}
