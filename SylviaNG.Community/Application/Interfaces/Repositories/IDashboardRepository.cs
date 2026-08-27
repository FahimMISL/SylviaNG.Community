namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    /// <summary>
    /// Cross-aggregate read queries backing Feature 8 (Dashboard Widgets) that have no
    /// existing home on a single aggregate's repository - team membership/supervision for
    /// an employee, an assigner's task-status breakdown, and which surveys an employee has
    /// already responded to. Everything else in the dashboard summaries is served by
    /// composing the existing per-feature services (see DashboardService).
    /// </summary>
    public interface IDashboardRepository
    {
        /// <summary>Distinct count of teams the employee belongs to, either as a member or as supervisor.</summary>
        Task<int> GetTeamCountForEmployeeAsync(long employeeId);

        /// <summary>True if the employee is the SupervisorId of at least one team (US-8.2 gate).</summary>
        Task<bool> IsSupervisorOfAnyTeamAsync(long employeeId);

        /// <summary>
        /// Task status breakdown for every task this employee assigned (team-scoped or individual -
        /// US-8.2's "everything I've assigned"), using the same Completed/Overdue derivation as
        /// TaskMapper.ComputeDerivedStatus.
        /// </summary>
        Task<(int Total, int InProgress, int Completed, int Overdue)> GetTaskStatsForAssignerAsync(long assignerEmployeeId);

        /// <summary>Ids of every survey this employee has already submitted a SurveyResponse for.</summary>
        Task<HashSet<long>> GetRespondedSurveyIdsAsync(long employeeId);
    }
}
