using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

        foreach (var entry in context.ChangeTracker.Entries<Audit>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = currentEmployeeId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = currentEmployeeId;
            }
        }
    }
}
