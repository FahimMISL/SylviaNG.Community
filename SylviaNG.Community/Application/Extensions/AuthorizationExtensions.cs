using Microsoft.AspNetCore.Authorization;

namespace SylviaNG.Community.Application.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Add community-specific authorization policies here
            });

            return services;
        }
    }
}
