using FluentAssertions;
using Moq;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Tests.Services;

public class DashboardServiceTests
{
    private readonly Mock<IDashboardRepository> _dashboardRepositoryMock;
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly Mock<ISurveyService> _surveyServiceMock;
    private readonly Mock<IRecognitionService> _recognitionServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IMarketplaceService> _marketplaceServiceMock;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _dashboardRepositoryMock = new Mock<IDashboardRepository>();
        _taskServiceMock = new Mock<ITaskService>();
        _surveyServiceMock = new Mock<ISurveyService>();
        _recognitionServiceMock = new Mock<IRecognitionService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _marketplaceServiceMock = new Mock<IMarketplaceService>();

        _service = new DashboardService(
            _dashboardRepositoryMock.Object,
            _taskServiceMock.Object,
            _surveyServiceMock.Object,
            _recognitionServiceMock.Object,
            _notificationServiceMock.Object,
            _marketplaceServiceMock.Object);
    }

    [Fact]
    public async Task GetEmployeeSummaryAsync_ShouldAggregateAcrossEveryDataSource()
    {
        // Arrange
        const long employeeId = 7;

        _dashboardRepositoryMock.Setup(r => r.GetTeamCountForEmployeeAsync(employeeId)).ReturnsAsync(3);
        _dashboardRepositoryMock.Setup(r => r.IsSupervisorOfAnyTeamAsync(employeeId)).ReturnsAsync(true);
        _dashboardRepositoryMock.Setup(r => r.GetRespondedSurveyIdsAsync(employeeId)).ReturnsAsync(new HashSet<long> { 1 });

        _taskServiceMock.Setup(s => s.GetMyPaginatedAsync(It.IsAny<TaskFilterRequest>(), employeeId))
            .ReturnsAsync(new PagedResult<TaskResponse>
            {
                Data = new List<TaskResponse>
                {
                    new() { TaskId = 1, Status = "Assigned" },
                    new() { TaskId = 2, Status = "InProgress" },
                    new() { TaskId = 3, Status = "Completed" },
                },
                TotalCount = 3
            });

        _recognitionServiceMock.Setup(s => s.GetPaginatedAsync(
                It.IsAny<PagedRequest>(), null, employeeId, employeeId, false))
            .ReturnsAsync(new PagedResult<RecognitionResponse> { TotalCount = 5 });

        _surveyServiceMock.Setup(s => s.GetPaginatedAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(new PagedResult<SurveyDetailResponse>
            {
                Data = new List<SurveyDetailResponse>
                {
                    new() { SurveyId = 1, Status = "Published" }, // already responded - excluded
                    new() { SurveyId = 2, Status = "Published" }, // not responded - counted
                    new() { SurveyId = 3, Status = "Draft" },     // not Published - excluded
                }
            });

        var recentNotifications = new List<NotificationResponse> { new() { NotificationId = 1, Title = "Welcome" } };
        _notificationServiceMock.Setup(s => s.GetPaginatedAsync(employeeId, It.IsAny<NotificationFilterRequest>()))
            .ReturnsAsync(new PagedResult<NotificationResponse> { Data = recentNotifications });

        // Act
        var result = await _service.GetEmployeeSummaryAsync(employeeId);

        // Assert
        result.TeamCount.Should().Be(3);
        result.IsSupervisor.Should().BeTrue();
        result.OpenTaskCount.Should().Be(2); // Assigned + InProgress, Completed excluded
        result.RecognitionsReceivedCount.Should().Be(5);
        result.PendingSurveyCount.Should().Be(1); // only survey 2
        result.RecentNotifications.Should().BeEquivalentTo(recentNotifications);
    }

    [Fact]
    public async Task GetSupervisorTaskOverviewAsync_ShouldMapRepositoryStats()
    {
        // Arrange
        const long supervisorId = 9;
        _dashboardRepositoryMock.Setup(r => r.GetTaskStatsForAssignerAsync(supervisorId))
            .ReturnsAsync((Total: 10, InProgress: 4, Completed: 3, Overdue: 2));

        // Act
        var result = await _service.GetSupervisorTaskOverviewAsync(supervisorId);

        // Assert
        result.Total.Should().Be(10);
        result.InProgress.Should().Be(4);
        result.Completed.Should().Be(3);
        result.Overdue.Should().Be(2);
    }

    [Fact]
    public async Task GetAdminSummaryAsync_ShouldAverageOnlyNonNullParticipationRates()
    {
        // Arrange
        _surveyServiceMock.Setup(s => s.GetPaginatedAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(new PagedResult<SurveyDetailResponse>
            {
                Data = new List<SurveyDetailResponse>
                {
                    new() { SurveyId = 1, Status = "Published" },
                    new() { SurveyId = 2, Status = "Published" },
                    new() { SurveyId = 3, Status = "Closed" }, // not Published - excluded from ActiveSurveyCount
                }
            });

        _surveyServiceMock.Setup(s => s.GetResultsAsync(1))
            .ReturnsAsync(new SurveyResultsResponse { SurveyId = 1, ParticipationRate = 80m });
        _surveyServiceMock.Setup(s => s.GetResultsAsync(2))
            .ReturnsAsync(new SurveyResultsResponse { SurveyId = 2, ParticipationRate = null }); // Department/Branch-scoped - excluded from average

        var recentRecognitions = new List<RecognitionResponse> { new() { RecognitionId = 1 } };
        _recognitionServiceMock.Setup(s => s.GetPaginatedAsync(
                It.IsAny<PagedRequest>(), null, null, null, true))
            .ReturnsAsync(new PagedResult<RecognitionResponse> { Data = recentRecognitions });

        _marketplaceServiceMock.Setup(s => s.GetListingsPagedAsync(
                It.Is<ListingFilterRequest>(r => r.ApprovalStatus == "Pending")))
            .ReturnsAsync(new PagedResult<ListingResponse> { TotalCount = 4 });

        // Act
        var result = await _service.GetAdminSummaryAsync();

        // Assert
        result.ActiveSurveyCount.Should().Be(2);
        result.AverageParticipationRate.Should().Be(80m);
        result.RecentRecognitions.Should().BeEquivalentTo(recentRecognitions);
        result.PendingListingCount.Should().Be(4);
    }

    [Fact]
    public async Task GetAdminSummaryAsync_WithNoComputableParticipationRate_ShouldReturnNullAverage()
    {
        // Arrange
        _surveyServiceMock.Setup(s => s.GetPaginatedAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(new PagedResult<SurveyDetailResponse>
            {
                Data = new List<SurveyDetailResponse> { new() { SurveyId = 1, Status = "Published" } }
            });

        _surveyServiceMock.Setup(s => s.GetResultsAsync(1))
            .ReturnsAsync(new SurveyResultsResponse { SurveyId = 1, ParticipationRate = null });

        _recognitionServiceMock.Setup(s => s.GetPaginatedAsync(
                It.IsAny<PagedRequest>(), null, null, null, true))
            .ReturnsAsync(new PagedResult<RecognitionResponse>());

        _marketplaceServiceMock.Setup(s => s.GetListingsPagedAsync(It.IsAny<ListingFilterRequest>()))
            .ReturnsAsync(new PagedResult<ListingResponse>());

        // Act
        var result = await _service.GetAdminSummaryAsync();

        // Assert
        result.AverageParticipationRate.Should().BeNull();
    }
}
