using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Domain.Constants;
using SylviaNG.Community.Infrastructure.Data;

namespace SylviaNG.Community.Infrastructure.BackgroundServices
{
    /// <summary>
    /// US-9.11: automatically transitions an Election's Status to Closed once its EndDate
    /// passes, so status-driven reads (the eligible-elections list, dashboards) reflect
    /// reality without HR having to close it manually. Votes past EndDate are already
    /// rejected by ElectionService.CastVoteAsync's date-window check regardless of this
    /// service's polling interval - this only keeps the persisted Status in sync.
    /// </summary>
    public class ElectionAutoCloseBackgroundService : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(5);

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ElectionAutoCloseBackgroundService> _logger;

        public ElectionAutoCloseBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ElectionAutoCloseBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Election auto-close background service starting...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CloseExpiredElectionsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Election auto-close tick failed.");
                }

                await Task.Delay(PollInterval, stoppingToken);
            }
        }

        private async Task CloseExpiredElectionsAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            var now = DateTime.UtcNow;
            var expiredElections = await dbContext.Elections
                .Where(e => ElectionStatus.Votable.Contains(e.Status) && e.EndDate.HasValue && e.EndDate.Value < now)
                .ToListAsync(stoppingToken);

            if (expiredElections.Count == 0)
                return;

            foreach (var election in expiredElections)
            {
                election.Status = ElectionStatus.Closed;
            }

            await dbContext.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Auto-closed {Count} expired election(s).", expiredElections.Count);
        }
    }
}
