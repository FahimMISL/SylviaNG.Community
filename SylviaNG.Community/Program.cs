using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using SylviaNG.Community.Application.Extensions;
using SylviaNG.Community.Hubs;
using SylviaNG.Community.Infrastructure.BackgroundServices;
using SylviaNG.Community.Infrastructure.Extensions;
using SylviaNG.Community.Middlewares;
using SylviaNG.Community.SharedKernel.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddGrpcServices(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, NotificationUserIdProvider>();
builder.Services.AddHostedService<ElectionAutoCloseBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // SignalR's default client sends credentialed (withCredentials: true) requests
        // for the hub negotiate handshake, which the CORS spec forbids combining with a
        // wildcard Access-Control-Allow-Origin. Reflecting the request origin via
        // SetIsOriginAllowed lets AllowCredentials() be added while still accepting any
        // origin, so hub connections (NotificationHub/FeedHub) can actually establish.
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddControllers();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SylviaNG Community API",
        Version = "v1",
        Description = "Community Management API with Keycloak Authentication"
    });

    // Add JWT Bearer Authentication
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document =>
    new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = new List<string>()
    });
});


// Add Keycloak Authentication (with a Development-only header-based fallback scheme
// so frontend apps without a real login flow yet can still exercise authorization)
builder.Services.AddKeycloakJwtAuthentication(builder.Configuration, builder.Environment);

builder.Services.AddAuthorizationPolicies();

// Add global authorization policy - all endpoints require authentication by default
builder.Services.AddControllers(options =>
{
    // Global authorization filter - all endpoints require authentication
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new LocalDateTimeJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new NullableLocalDateTimeJsonConverter());
    });




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

var uploadsRootPath = Path.Combine(app.Environment.ContentRootPath, builder.Configuration["FileStorage:LocalRootPath"] ?? "wwwroot/uploads");
Directory.CreateDirectory(uploadsRootPath); // PhysicalFileProvider throws if the directory doesn't exist yet

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsRootPath),
    RequestPath = "/uploads"
});

app.UseMiddleware<ResponseWrappingMiddleware>();

app.UseAuthentication();
app.UseMiddleware<EmployeeIdentityEnrichmentMiddleware>();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();
app.MapHub<NotificationHub>("/community/hubs/notifications");
app.MapHub<FeedHub>("/community/hubs/feed");
app.MapHub<MessengerHub>("/community/hubs/messenger");

app.Run();
