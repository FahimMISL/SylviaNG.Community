using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Externals;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Infrastructure.Authentication;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.Infrastructure.Interceptors;
using SylviaNG.Community.Infrastructure.Repositories;
using SylviaNG.Community.Infrastructure.Services;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Utils;

namespace SylviaNG.Community.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add your infrastructure services here

            var databaseProvider = configuration["Database:Provider"];
            var connectionString = configuration["Database:ConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Database connection string is not configured.");

            // Initialize timezone from configuration
            var timezoneId = configuration["RegionalSettings:TimezoneId"]
                ?? throw new InvalidOperationException("RegionalSettings:TimezoneId is not configured.");
            DateTimeUtility.Initialize(timezoneId);

            // Configure Finbuckle Multi-Tenant with Claim strategy (extracts tenant_id from JWT)
            services.AddMultiTenant<MultiTenancy.TenantInfo>()
                .WithClaimStrategy("tenant_id")  // Extract tenant from JWT claim 'tenant_id'
                .WithInMemoryStore(options =>
                {
                    // Default tenant for fallback
                    options.IsCaseSensitive = false;
                });

            // Register Audit Infrastructure (database-agnostic)
            services.AddHttpContextAccessor();
            services.AddSingleton<UtcDateTimeInterceptor>();
            services.AddSingleton<AuditInterceptor>();

            // Configure database provider with audit interceptor
            services.AddDbContext<ApplicationDBContext>((sp, options) =>
            {
                var provider = NormalizeDatabaseProvider(databaseProvider);

                switch (provider)
                {
                    case "postgresql":
                        options.UseNpgsql(connectionString);
                        break;
                    case "sqlserver":
                        options.UseSqlServer(connectionString);
                        break;
                    case "oracle":
                        options.UseOracle(connectionString);
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Unsupported database provider: {databaseProvider}. Supported providers: PostgreSQL, SqlServer, Oracle.");
                }

                // Apply audit interceptor once (works with any database)
                options.AddInterceptors(sp.GetRequiredService<UtcDateTimeInterceptor>(), sp.GetRequiredService<AuditInterceptor>());
            });

            // Register your repositories here
            // Adding DI of repositories
            services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
            services.AddScoped<IEmployeeKeycloakAccountRepository, EmployeeKeycloakAccountRepository>();

            // Real Keycloak user provisioning for HR/Admin-created employee login credentials
            // (distinct from the local Credential/InMemoryCredentialRepository login system below).
            services.AddHttpClient<IKeycloakAdminClient, KeycloakAdminClient>();

            // Organization master data (Department/Branch/Designation/Role)
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            // Module 2 - Profile tagging
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IEmployeeSkillRepository, EmployeeSkillRepository>();
            services.AddScoped<IInterestRepository, InterestRepository>();
            services.AddScoped<IEmployeeInterestRepository, EmployeeInterestRepository>();
            services.AddScoped<IBadgeRepository, BadgeRepository>();
            services.AddScoped<IEmployeeBadgeRepository, EmployeeBadgeRepository>();
            services.AddScoped<IEmployeeContactLinkRepository, EmployeeContactLinkRepository>();

            // Module 5 - Recognition
            services.AddScoped<IRecognitionRepository, RecognitionRepository>();
            services.AddScoped<IRecognitionBadgeRepository, RecognitionBadgeRepository>();
            services.AddScoped<IRecognitionReactionRepository, RecognitionReactionRepository>();
            services.AddScoped<IRecognitionCommentRepository, RecognitionCommentRepository>();

            // Module 3 - Notifications
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();
            services.AddSingleton<INotificationBroadcaster, SignalRNotificationBroadcaster>();
            services.AddSingleton<IFeedBroadcaster, SignalRFeedBroadcaster>();

            // Module 9 - System/Admin
            services.AddScoped<IDashboardPreferenceRepository, DashboardPreferenceRepository>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IFileStorageRepository, FileStorageRepository>();

            // Module 4 - Social Feed
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostAttachmentRepository, PostAttachmentRepository>();
            services.AddScoped<IPostCommentRepository, PostCommentRepository>();
            services.AddScoped<IPostReactionRepository, PostReactionRepository>();
            services.AddScoped<ICommentReactionRepository, CommentReactionRepository>();
            services.AddScoped<IMentionRepository, MentionRepository>();
            services.AddScoped<IPollRepository, PollRepository>();
            services.AddScoped<IPollOptionRepository, PollOptionRepository>();
            services.AddScoped<IPollVoteRepository, PollVoteRepository>();
            services.AddScoped<IContentReportRepository, ContentReportRepository>();

            // Module 6 - Survey & Feedback
            services.AddScoped<ISurveyRepository, SurveyRepository>();
            services.AddScoped<ISurveyAudienceRepository, SurveyAudienceRepository>();
            services.AddScoped<ISurveyQuestionRepository, SurveyQuestionRepository>();
            services.AddScoped<ISurveyOptionRepository, SurveyOptionRepository>();
            services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
            services.AddScoped<ISurveyAnswerRepository, SurveyAnswerRepository>();

            // Module 7 - Marketplace
            services.AddScoped<IListingRepository, ListingRepository>();
            services.AddScoped<IListingImageRepository, ListingImageRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IConversationParticipantRepository, ConversationParticipantRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMarketplaceReportRepository, MarketplaceReportRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IReviewImageRepository, ReviewImageRepository>();

            // Module 8 - Task Management
            services.AddScoped<IRecurringTaskRepository, RecurringTaskRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();
            services.AddScoped<ITaskAttachmentRepository, TaskAttachmentRepository>();
            services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();

            // Module 10 - Voting/Election
            services.AddScoped<IElectionRepository, ElectionRepository>();
            services.AddScoped<IElectionAudienceTargetRepository, ElectionAudienceTargetRepository>();
            services.AddScoped<IElectionCandidateRepository, ElectionCandidateRepository>();
            services.AddScoped<IElectionVoteRepository, ElectionVoteRepository>();

            // Module 4 - Social Feed (Interest-Based Groups)
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
            services.AddScoped<IGroupJoinRequestRepository, GroupJoinRequestRepository>();

            // No database is provisioned yet - login credentials are a static in-memory list
            // (see InMemoryCredentialRepository) instead of an EF-backed table.
            services.AddSingleton<ICredentialRepository, InMemoryCredentialRepository>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Current-user resolution (JWT claims in production; see
            // DevHeaderAuthenticationHandler for the Development-only fallback)
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Local JWT issuance for the admin UI's login page (see AuthController/AuthService)
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }

        private static string NormalizeDatabaseProvider(string? provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentNullException(nameof(provider), "Database provider is not specified.");

            return provider.Trim().ToLowerInvariant();
        }
    }
}
