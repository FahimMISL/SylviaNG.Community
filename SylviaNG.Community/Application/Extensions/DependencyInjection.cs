using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Services;
using System.Reflection;

namespace SylviaNG.Community.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Add your application services here

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );

            services.AddFluentValidationAutoValidation()
               .AddValidatorsFromAssembly(typeof(Program).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Register your services here
            // Adding DI of services

            #region Services -> Business Classes
            // Add community-specific services here

            services.AddScoped<IAnnouncementService, AnnouncementService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IEmployeeCredentialService, EmployeeCredentialService>();

            // Organization master data (Department/Branch/Designation/Role)
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<IRoleService, RoleService>();

            // Module 2 - Profile tagging
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IInterestService, InterestService>();
            services.AddScoped<IBadgeService, BadgeService>();

            // Module 5 - Recognition
            services.AddScoped<IRecognitionService, RecognitionService>();

            // Module 3 - Notifications
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INotificationPreferenceService, NotificationPreferenceService>();

            // Module 9 - System/Admin
            services.AddScoped<IDashboardPreferenceService, DashboardPreferenceService>();
            services.AddScoped<IActivityLogService, ActivityLogService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IFileStorageService, FileStorageService>();

            // Module 4 - Social Feed
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostAttachmentService, PostAttachmentService>();
            services.AddScoped<IPostCommentService, PostCommentService>();
            services.AddScoped<IPostReactionService, PostReactionService>();
            services.AddScoped<ICommentReactionService, CommentReactionService>();
            services.AddScoped<IMentionService, MentionService>();
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IContentReportService, ContentReportService>();

            // Module 6 - Survey & Feedback
            services.AddScoped<ISurveyService, SurveyService>();

            // Module 7 - Marketplace
            services.AddScoped<IMarketplaceService, MarketplaceService>();

            // Module 8 - Task Management
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IRecurringTaskService, RecurringTaskService>();

            // Module 10 - Voting/Election
            services.AddScoped<IElectionService, ElectionService>();

            // Module 4 - Social Feed (Interest-Based Groups)
            services.AddScoped<IGroupService, GroupService>();

            // Provide access to HttpContext for request metadata enrichment
            services.AddHttpContextAccessor();
            #endregion

            return services;
        }
    }
}
