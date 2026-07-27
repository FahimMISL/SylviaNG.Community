using SylviaNG.Community.Application.Interfaces.Externals;
using SylviaNG.Community.Grpc.Generated.Core;
using SylviaNG.Community.Infrastructure.Services;

namespace SylviaNG.Community.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for gRPC service registration
    /// </summary>
    public static class GrpcExtensions
    {
        /// <summary>
        /// Registers gRPC client services and channels
        /// </summary>
        public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
        {
            // When the real Core service isn't reachable (e.g. local development), a stub
            // implementation can stand in so department/designation/site/grade names still resolve.
            if (configuration.GetValue<bool>("GrpcServices:CoreService:UseStub"))
            {
                services.AddScoped<ICoreGrpcClient, StubCoreGrpcClient>();
                return services;
            }

            // ── gRPC: Core Service Channel ───────────────────────────────────────────
            var coreServiceUrl = configuration["GrpcServices:CoreService:Url"] ?? "http://localhost:7000";

            services.AddGrpcClient<CoreService.CoreServiceClient>(options =>
            {
                options.Address = new Uri(coreServiceUrl);
            });

            services.AddScoped<ICoreGrpcClient, CoreGrpcClient>();

            return services;
        }
    }
}
