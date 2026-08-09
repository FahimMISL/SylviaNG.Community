using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SylviaNG.Community.Infrastructure.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SylviaNG.Community.Application.Extensions
{
    public static class AuthenticationExtensions
    {
        private const string SmartScheme = "Smart";
        private const string LocalScheme = "Local";

        public static IServiceCollection AddKeycloakJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            var keycloakSection = configuration.GetSection("Keycloak");
            if (!keycloakSection.Exists())
            {
                throw new ArgumentException("Keycloak configuration section is missing in appsettings.json");
            }

            var authority = keycloakSection.GetValue<string>("Authority") ?? throw new ArgumentNullException("Keycloak:Authority");
            var clientId = keycloakSection.GetValue<string>("ClientId") ?? throw new ArgumentNullException("Keycloak:ClientId");
            var requireHttps = keycloakSection.GetValue<bool>("RequireHttpsMetadata", false);

            var jwtSection = configuration.GetSection("Jwt");
            if (!jwtSection.Exists())
            {
                throw new ArgumentException("Jwt configuration section is missing in appsettings.json");
            }

            var localIssuer = jwtSection.GetValue<string>("Issuer") ?? throw new ArgumentNullException("Jwt:Issuer");
            var localAudience = jwtSection.GetValue<string>("Audience") ?? throw new ArgumentNullException("Jwt:Audience");
            var localSigningKey = jwtSection.GetValue<string>("SigningKey") ?? throw new ArgumentNullException("Jwt:SigningKey");

            var isDevelopment = environment.IsDevelopment();

            var authBuilder = services.AddAuthentication(options =>
            {
                // Every request is routed through the "Smart" policy scheme (see below),
                // which decides between the Keycloak scheme, the locally-issued-JWT scheme
                // ("Local" - see JwtTokenGenerator/AuthController), and, in Development only,
                // the dev-header fallback scheme.
                options.DefaultAuthenticateScheme = SmartScheme;
                options.DefaultChallengeScheme = SmartScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = authority;
                options.Audience = clientId;
                options.RequireHttpsMetadata = requireHttps;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = clientId,
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = BuildJwtBearerEvents();
            })
            .AddJwtBearer(LocalScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = localIssuer,
                    ValidateAudience = true,
                    ValidAudience = localAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(localSigningKey)),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = BuildJwtBearerEvents();
            });

            if (isDevelopment)
            {
                authBuilder.AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, DevHeaderAuthenticationHandler>(
                    DevHeaderAuthenticationHandler.SchemeName, _ => { });
            }

            authBuilder.AddPolicyScheme(SmartScheme, "Keycloak, Local, or Dev Header", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                    {
                        var issuer = TryPeekUnvalidatedIssuer(authHeader.ToString());
                        if (issuer == localIssuer)
                            return LocalScheme;

                        return JwtBearerDefaults.AuthenticationScheme;
                    }

                    // Browsers can't set the Authorization header on a WebSocket upgrade, so the
                    // SignalR client appends ?access_token=<token> instead - check for that on the
                    // notifications hub path before falling through to the dev/Keycloak default.
                    if (context.Request.Path.StartsWithSegments("/community/hubs/notifications")
                        && context.Request.Query.TryGetValue("access_token", out var queryToken))
                    {
                        var issuer = TryPeekUnvalidatedIssuer($"Bearer {queryToken}");
                        if (issuer == localIssuer)
                            return LocalScheme;

                        return JwtBearerDefaults.AuthenticationScheme;
                    }

                    // No bearer token: in Development, let the dev-header persona-switcher
                    // authenticate; elsewhere fall through to Keycloak's default challenge.
                    return isDevelopment ? DevHeaderAuthenticationHandler.SchemeName : JwtBearerDefaults.AuthenticationScheme;
                };
            });

            return services;
        }

        /// <summary>
        /// Peeks at a bearer token's "iss" claim WITHOUT validating its signature, purely to
        /// decide which registered JwtBearer scheme should perform the real (signature-checked)
        /// validation. Never used for anything security-sensitive - a forged issuer claim here
        /// only routes the request to a scheme that will then reject it during real validation.
        /// </summary>
        private static string? TryPeekUnvalidatedIssuer(string authorizationHeaderValue)
        {
            try
            {
                const string prefix = "Bearer ";
                if (!authorizationHeaderValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return null;

                var token = authorizationHeaderValue[prefix.Length..].Trim();
                var parts = token.Split('.');
                if (parts.Length < 2)
                    return null;

                var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
                using var document = JsonDocument.Parse(payloadJson);
                return document.RootElement.TryGetProperty("iss", out var issProperty) ? issProperty.GetString() : null;
            }
            catch
            {
                return null;
            }
        }

        private static byte[] Base64UrlDecode(string input)
        {
            var padded = input.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return Convert.FromBase64String(padded);
        }

        private static JwtBearerEvents BuildJwtBearerEvents()
        {
            return new JwtBearerEvents
            {
                // Browsers cannot set the Authorization header on a WebSocket upgrade request,
                // so the SignalR JS client's accessTokenFactory instead appends
                // ?access_token=<token> to the connection URL for the WebSocket transport.
                // Ordinary REST calls and the hub's initial HTTP negotiate POST are unaffected
                // since they still carry the real header.
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/community/hubs/notifications"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("Token validated successfully");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        hasError = true,
                        decentMessage = "Unauthorized. Authentication is required.",
                        errorDetails = context.ErrorDescription ?? "Invalid or missing authentication token.",
                        content = (object?)null
                    };

                    var json = JsonSerializer.Serialize(response);
                    return context.Response.WriteAsync(json);
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        hasError = true,
                        decentMessage = "Forbidden. You don't have permission to access this resource.",
                        errorDetails = "Access denied. Insufficient permissions.",
                        content = (object?)null
                    };

                    var json = JsonSerializer.Serialize(response);
                    return context.Response.WriteAsync(json);
                }
            };
        }
    }
}
