using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    /// <summary>
    /// Static in-memory demo accounts for the admin UI's login page - no database is
    /// provisioned yet, so credentials live here instead of a table. One account per role
    /// (Employee/Supervisor/HR/Admin), EmployeeIds matching the seeded rows in
    /// EmployeeConfiguration so the logged-in identity lines up with existing employee data.
    /// Demo passwords: Employee@123 / Supervisor@123 / HR@123 / Admin@123 (hashes below are
    /// BCrypt of those, workFactor 11). Swap this out for a real database-backed
    /// ICredentialRepository once user storage is provisioned.
    /// </summary>
    public class InMemoryCredentialRepository : ICredentialRepository
    {
        private static readonly IReadOnlyList<Credential> Accounts = new List<Credential>
        {
            new()
            {
                Id = 1,
                Username = "ayesha.rahman",
                PasswordHash = "$2a$11$uMK8aOaK34AQ4xNOfz31i.2VNUbt/F6b72dfvWVqZcUHB2fvwfCfi",
                DisplayName = "Ayesha Rahman",
                Role = "Employee",
                EmployeeId = 1,
                IsActive = true,
            },
            new()
            {
                Id = 2,
                Username = "tanvir.hasan",
                PasswordHash = "$2a$11$CwFOdIlZTr8p6.UgRv/c5Ou/GNfb8.d0NMsIe.CA41KEPTN1HUXgS",
                DisplayName = "Tanvir Hasan",
                Role = "Supervisor",
                EmployeeId = 2,
                IsActive = true,
            },
            new()
            {
                Id = 3,
                Username = "farhana.akter",
                PasswordHash = "$2a$11$HdGzzEhYrMFALZ61A8PXZ.qUd30ZeYcKDrzqRxa85Ht4QiVZiHEgS",
                DisplayName = "Farhana Akter",
                Role = "HR",
                EmployeeId = 3,
                IsActive = true,
            },
            new()
            {
                Id = 4,
                Username = "admin",
                PasswordHash = "$2a$11$ClD2UNJCElOaCcmupUN25.aboAas880iVrsFhaKaNOVBgJmtsZH8u",
                DisplayName = "System Admin",
                Role = "Admin",
                EmployeeId = null,
                IsActive = true,
            },
        };

        // Accounts is a static IReadOnlyList (immutable structure), but the Credential
        // objects it holds are mutable - lock guards concurrent PasswordHash writes across
        // requests, since this list is shared process-wide.
        private static readonly object PasswordLock = new();

        public Task<Credential?> GetByUsernameAsync(string username)
        {
            var match = Accounts.FirstOrDefault(c => c.Username == username && c.IsActive);
            return System.Threading.Tasks.Task.FromResult(match);
        }

        public System.Threading.Tasks.Task UpdatePasswordHashAsync(string username, string newPasswordHash)
        {
            lock (PasswordLock)
            {
                var credential = Accounts.FirstOrDefault(c => c.Username == username && c.IsActive);
                if (credential != null)
                {
                    credential.PasswordHash = newPasswordHash;
                }
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
