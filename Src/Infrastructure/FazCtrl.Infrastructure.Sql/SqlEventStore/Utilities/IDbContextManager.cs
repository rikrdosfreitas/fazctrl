using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FazCtrl.Infrastructure.Sql.SqlEventStore.Utilities
{
    public interface IDbContextManager : IDbContextRegistry
    {
        bool IsInTransaction { get; }

        Task ExecuteInTransactionAsync(DbContext context, Func<CancellationToken, Task> operation, CancellationToken token = default);
    }
}
