using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Infrastructure.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private const string EmployeeIdClaimType = "employee_id";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Stamp(DbContext? context)
    {
        if (context == null) return;

        var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst(EmployeeIdClaimType)?.Value;
        long? currentEmployeeId = long.TryParse(claimValue, out var employeeId) ? employeeId : null;
        var now = DateTime.UtcNow;

        // Same 3-tier resolution (JWT claim -> Finbuckle -> empty) already used to filter reads
        // (ApplicationDBContext.CurrentTenantId) - reused here so every new row is stamped with the
        // tenant that will actually be able to see it. Left at Audit.TenantId's "default_tenant"
        // default when no tenant context resolves (e.g. background jobs / Kafka consumers with no
        // HttpContext), rather than writing an empty string.
        var currentTenantId = (context as ApplicationDBContext)?.CurrentTenantId;

        foreach (var entry in context.ChangeTracker.Entries<Audit>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = currentEmployeeId;
                if (!string.IsNullOrEmpty(currentTenantId))
                    entry.Entity.TenantId = currentTenantId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = currentEmployeeId;
            }
        }
    }
}
