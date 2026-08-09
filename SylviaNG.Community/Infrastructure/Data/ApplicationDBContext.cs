using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Events;

namespace SylviaNG.Community.Infrastructure.Data
{
    public class ApplicationDBContext : DbContext
    {
        private readonly IMultiTenantContextAccessor<MultiTenancy.TenantInfo>? _multiTenantContextAccessor;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDBContext(
            DbContextOptions<ApplicationDBContext> options,
            IMultiTenantContextAccessor<MultiTenancy.TenantInfo>? multiTenantContextAccessor = null,
            IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the current tenant ID with priority order:
        /// 1. JWT claims from HttpContext (HTTP requests)
        /// 2. Finbuckle TenantInfo
        /// 3. Empty string fallback
        /// This property is evaluated at query execution time for runtime filtering.
        /// </summary>
        public string CurrentTenantId
        {
            get
            {
                // Priority 1: JWT claims from HTTP request
                if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    var tenantClaim = _httpContextAccessor.HttpContext.User.FindFirst("tenant_id");
                    if (tenantClaim != null && !string.IsNullOrEmpty(tenantClaim.Value))
                    {
                        return tenantClaim.Value;
                    }
                }

                // Priority 2: Finbuckle TenantInfo
                if (_multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Identifier != null)
                {
                    return _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Identifier;
                }

                // Priority 3: Fallback
                return string.Empty;
            }
        }

        #region Tables

        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }

        // Organization master data (Department/Branch/Designation/Role)
        public DbSet<Department> Departments { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Role> Roles { get; set; }

        // Module 2 - Profile tagging
        public DbSet<Skill> Skills { get; set; }
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<EmployeeInterest> EmployeeInterests { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<EmployeeBadge> EmployeeBadges { get; set; }
        public DbSet<EmployeeContactLink> EmployeeContactLinks { get; set; }

        // Module 5 - Recognition
        public DbSet<Recognition> Recognitions { get; set; }
        public DbSet<RecognitionBadge> RecognitionBadges { get; set; }
        public DbSet<RecognitionReaction> RecognitionReactions { get; set; }
        public DbSet<RecognitionComment> RecognitionComments { get; set; }

        // Module 3 - Notifications
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }

        // Module 9 - System/Admin
        public DbSet<DashboardPreference> DashboardPreferences { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<FileStorage> FileStorages { get; set; }

        // Module 4 - Social Feed
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostAttachment> PostAttachments { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<CommentReaction> CommentReactions { get; set; }
        public DbSet<Mention> Mentions { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollOption> PollOptions { get; set; }
        public DbSet<PollVote> PollVotes { get; set; }
        public DbSet<ContentReport> ContentReports { get; set; }

        // Module 6 - Survey & Feedback
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyAudience> SurveyAudiences { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<SurveyOption> SurveyOptions { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<SurveyAnswer> SurveyAnswers { get; set; }

        // Module 7 - Marketplace
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingImage> ListingImages { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MarketplaceReport> MarketplaceReports { get; set; }

        // Module 8 - Task Management ("Task" is qualified to avoid ambiguity with System.Threading.Tasks.Task)
        public DbSet<RecurringTask> RecurringTasks { get; set; }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskAttachment> TaskAttachments { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<TaskTag> TaskTags { get; set; }
        public DbSet<TaskTagMapping> TaskTagMappings { get; set; }

        // Module 10 - Voting/Election
        public DbSet<Election> Elections { get; set; }
        public DbSet<ElectionAudienceTarget> ElectionAudienceTargets { get; set; }
        public DbSet<ElectionCandidate> ElectionCandidates { get; set; }
        public DbSet<ElectionVote> ElectionVotes { get; set; }

        // Module 4 - Social Feed (Interest-Based Groups)
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupJoinRequest> GroupJoinRequests { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDBContext).Assembly);

            // Ignore DomainEvents collection (used for in-memory event handling, not database persistence)
            modelBuilder.Ignore<DomainEvent>();
        }
    }
}
