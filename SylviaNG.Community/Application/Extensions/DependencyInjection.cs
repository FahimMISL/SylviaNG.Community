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

            // Provide access to HttpContext for request metadata enrichment
            services.AddHttpContextAccessor();
            #endregion

            return services;
        }
    }
}
