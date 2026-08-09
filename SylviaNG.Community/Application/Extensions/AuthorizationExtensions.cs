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
            });

            return services;
        }
    }
}
