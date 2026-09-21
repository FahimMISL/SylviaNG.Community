using Microsoft.AspNetCore.Authorization;

namespace SylviaNG.Community.Application.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Employee Profiles & Directory (Feature 1): HR/Admin-only actions
                // (add employee, management list, deactivate).
                options.AddPolicy("HRAdminOnly", policy => policy.RequireRole("HR", "Admin"));

                // Surveys (Feature 5): authoring actions (create/delete) are HR-only - Admin is a
                // system account for platform administration, not survey content ownership, so it
                // shouldn't create or permanently delete survey content it can't itself take part in.
                options.AddPolicy("HROnly", policy => policy.RequireRole("HR"));
            });

            return services;
        }
    }
}
