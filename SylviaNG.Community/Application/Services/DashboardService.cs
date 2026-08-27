using SylviaNG.Community.Application.Features.Dashboard.Models;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ITaskService _taskService;
        private readonly ISurveyService _surveyService;
        private readonly IRecognitionService _recognitionService;
        private readonly INotificationService _notificationService;
        private readonly IMarketplaceService _marketplaceService;

        public DashboardService(
            IDashboardRepository dashboardRepository,
            ITaskService taskService,
            ISurveyService surveyService,
            IRecognitionService recognitionService,
            INotificationService notificationService,
            IMarketplaceService marketplaceService)
        {
            _dashboardRepository = dashboardRepository;
            _taskService = taskService;
            _surveyService = surveyService;
            _recognitionService = recognitionService;
            _notificationService = notificationService;
            _marketplaceService = marketplaceService;
        }

        public async Task<EmployeeDashboardSummaryResponse> GetEmployeeSummaryAsync(long employeeId)
        {
            var teamCount = await _dashboardRepository.GetTeamCountForEmployeeAsync(employeeId);
            var isSupervisor = await _dashboardRepository.IsSupervisorOfAnyTeamAsync(employeeId);

            // "Open" = not yet marked Completed, regardless of overdue/due-soon standing (US-8.1
            // just wants a single count - the breakdown by standing is what US-8.2's card is for).
            var myTasks = await _taskService.GetMyPaginatedAsync(new TaskFilterRequest { Page = 1, PageSize = 100 }, employeeId);
            var openTaskCount = myTasks.Data.Count(t => !string.Equals(t.Status, "Completed", StringComparison.OrdinalIgnoreCase));

            var recognitionsReceived = await _recognitionService.GetPaginatedAsync(
                new PagedRequest { Page = 1, PageSize = 1 },
                recipientId: employeeId,
                viewerEmployeeId: employeeId,
                viewerIsHrAdmin: false);

            var pendingSurveyCount = await GetPendingSurveyCountAsync(employeeId);

            var recentNotifications = await _notificationService.GetPaginatedAsync(
                employeeId,
                new NotificationFilterRequest { Page = 1, PageSize = 5 });

            return new EmployeeDashboardSummaryResponse
            {
                TeamCount = teamCount,
                PendingSurveyCount = pendingSurveyCount,
                RecognitionsReceivedCount = recognitionsReceived.TotalCount,
                OpenTaskCount = openTaskCount,
                RecentNotifications = recentNotifications.Data,
                IsSupervisor = isSupervisor
            };
        }

        private async Task<int> GetPendingSurveyCountAsync(long employeeId)
        {
            // The backend has no status filter on the survey list endpoint (see SurveyService) -
            // fetch up to the server-side page cap and bucket here, same limitation SurveysComponent
            // already lives with client-side, just computed once server-side instead of per client.
            var surveys = await _surveyService.GetPaginatedAsync(new PagedRequest { Page = 1, PageSize = 100 });
            var respondedIds = await _dashboardRepository.GetRespondedSurveyIdsAsync(employeeId);

            return surveys.Data.Count(s =>
                string.Equals(s.Status, "Published", StringComparison.OrdinalIgnoreCase)
                && !respondedIds.Contains(s.SurveyId));
        }

        public async Task<SupervisorTaskOverviewResponse> GetSupervisorTaskOverviewAsync(long supervisorEmployeeId)
        {
            var stats = await _dashboardRepository.GetTaskStatsForAssignerAsync(supervisorEmployeeId);

            return new SupervisorTaskOverviewResponse
            {
                Total = stats.Total,
                InProgress = stats.InProgress,
                Completed = stats.Completed,
                Overdue = stats.Overdue
            };
        }

        public async Task<AdminDashboardSummaryResponse> GetAdminSummaryAsync()
        {
            var surveysPage = await _surveyService.GetPaginatedAsync(new PagedRequest { Page = 1, PageSize = 100 });
            var publishedSurveys = surveysPage.Data
                .Where(s => string.Equals(s.Status, "Published", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Reuses SurveyResultsGet's own participation-rate calculation (only populated for
            // EntireCompany-scoped surveys - see SurveyResultsResponse) instead of re-deriving
            // audience math here.
            var participationRates = new List<decimal>();
            foreach (var survey in publishedSurveys)
            {
                var results = await _surveyService.GetResultsAsync(survey.SurveyId);
                if (results.ParticipationRate.HasValue)
                    participationRates.Add(results.ParticipationRate.Value);
            }

            var recentRecognitions = await _recognitionService.GetPaginatedAsync(
                new PagedRequest { Page = 1, PageSize = 5, SortBy = "CreatedAt", SortDirection = "desc" },
                viewerIsHrAdmin: true);

            var pendingListings = await _marketplaceService.GetListingsPagedAsync(
                new ListingFilterRequest { ApprovalStatus = "Pending", Page = 1, PageSize = 1 });

            return new AdminDashboardSummaryResponse
            {
                ActiveSurveyCount = publishedSurveys.Count,
                AverageParticipationRate = participationRates.Count > 0 ? participationRates.Average() : null,
                RecentRecognitions = recentRecognitions.Data,
                PendingListingCount = pendingListings.TotalCount
            };
        }
    }
}
