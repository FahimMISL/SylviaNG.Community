using SylviaNG.Community.Infrastructure.Data;

namespace SylviaNG.Community.SharedKernel.Generic
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        ApplicationDBContext Context { get; }
    }
}
