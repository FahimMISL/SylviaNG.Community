using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Infrastructure.Data;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDBContext _context;

        public DashboardRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<int> GetTeamCountForEmployeeAsync(long employeeId)
        {
            var memberTeamIds = _context.TeamMembers
                .Where(tm => tm.EmployeeId == employeeId && tm.IsActive)
                .Select(tm => tm.TeamId);

            var supervisedTeamIds = _context.Teams
                .Where(t => t.SupervisorId == employeeId && t.IsActive)
                .Select(t => t.TeamId);

            return await memberTeamIds.Union(supervisedTeamIds).CountAsync();
        }

        public Task<bool> IsSupervisorOfAnyTeamAsync(long employeeId)
        {
            return _context.Teams.AnyAsync(t => t.SupervisorId == employeeId && t.IsActive);
        }

        public async Task<(int Total, int InProgress, int Completed, int Overdue)> GetTaskStatsForAssignerAsync(long assignerEmployeeId)
        {
            var tasks = await _context.Set<TaskEntity>()
                .Where(t => t.AssignedBy == assignerEmployeeId)
                .Select(t => new { t.TaskStatus, t.DueDate, t.ReminderDays })
                .ToListAsync();

            var now = DateTime.UtcNow;
            var total = tasks.Count;
            var completed = tasks.Count(t => string.Equals(t.TaskStatus, "Completed", StringComparison.OrdinalIgnoreCase));
            var inProgress = tasks.Count(t => string.Equals(t.TaskStatus, "InProgress", StringComparison.OrdinalIgnoreCase));

            // Mirrors TaskMapper.ComputeDerivedStatus: Overdue only applies to a task that isn't
            // already Completed and whose due date has passed.
            var overdue = tasks.Count(t =>
                !string.Equals(t.TaskStatus, "Completed", StringComparison.OrdinalIgnoreCase)
                && t.DueDate.HasValue
                && t.DueDate.Value < now);

            return (total, inProgress, completed, overdue);
        }

        public async Task<HashSet<long>> GetRespondedSurveyIdsAsync(long employeeId)
        {
            var ids = await _context.SurveyResponses
                .Where(r => r.EmployeeId == employeeId)
                .Select(r => r.SurveyId)
                .ToListAsync();

            return ids.ToHashSet();
        }
    }
}
