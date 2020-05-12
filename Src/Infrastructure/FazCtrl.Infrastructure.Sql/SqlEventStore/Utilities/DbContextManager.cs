using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FazCtrl.Infrastructure.Sql.SqlEventStore.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FazCtrl.Infrastructure.Sql.SqlEventStore.Utilities
{
    public class DbContextManager : IDbContextManager, IDisposable
    {
        private readonly ContextSettings _contextSettings;
        private DbContextSet _context;
        private TransactionDbContext _db;

        public DbContextManager(ContextSettings contextSettings)
        {
            _contextSettings = contextSettings;
            _context = new DbContextSet();
        }

        public bool IsInTransaction => _db?.Database.CurrentTransaction != null;

        public async Task ExecuteInTransactionAsync(DbContext context, Func<CancellationToken, Task> operation, CancellationToken token = default)
        {
            _context.ValidateContext(context);

            /* Connection Resilience Strategy: "Manually track the transaction" reference: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency */
            var connection = context.Database.GetDbConnection();
            using (_db = CreateTransactionContext(connection))
            {
                var strategy = _db.Database.CreateExecutionStrategy();

                var transaction = new TransactionRow { Id = Guid.NewGuid() };
                _db.Transactions.Add(transaction);

                await strategy.ExecuteInTransactionAsync(
                    async () => await _context.SaveAsync(token),
                    verifySucceeded: async () => await _db.Transactions.AsNoTracking().AnyAsync(t => t.Id == transaction.Id, token));

                _context.AcceptAllChanges();

                _db.Transactions.Remove(transaction);
                _db.SaveChanges();
            }

            _db = null;
        }

        private TransactionDbContext CreateTransactionContext(DbConnection connection)
        {
            var options = new DbContextOptionsBuilder<TransactionDbContext>()
                .UseSqlServer(connection, op =>
                {
                    op.MigrationsHistoryTable("", _contextSettings.Schema);
                    op.MigrationsAssembly(_contextSettings.MigrationAssembly);
                    op.EnableRetryOnFailure();
                }).Options;

            var context = new TransactionDbContext(options);

            _context.RegisterContext(context);

            return context;
        }

        public IDbContextManager RegisterContext(DbContext context)
        {
            _context.RegisterContext(context);

            return this;
        }

        #region class

        protected class DbContextSet : IDisposable
        {
            private readonly List<DbContext> _contexts = new List<DbContext>();

            public async Task SaveAsync(CancellationToken token = default)
            {
                var transaction = _contexts
                    .Single(c => c.Database.CurrentTransaction != null)
                    .Database.CurrentTransaction.GetDbTransaction();

                foreach (var context in _contexts)
                {
                    if (context.Database.CurrentTransaction == null)
                    {
                        context.Database.UseTransaction(transaction);
                    }

                    await context.SaveChangesAsync(acceptAllChangesOnSuccess: false, cancellationToken: token);
                }
            }

            public void AcceptAllChanges()
            {
                foreach (var context in _contexts)
                {
                    context.ChangeTracker.AcceptAllChanges();
                }
            }

            public void RegisterContext(DbContext context)
            {
                if (!_contexts.Contains(context))
                {
                    _contexts.Add(context);
                }
            }

            public void ValidateContext(DbContext context)
            {
                if (!_contexts.Contains(context))
                {
                    throw new InvalidOperationException("Database Context not found: " + context.GetType().Name);
                }
            }


            #region IDisposable
            public void Dispose()
            {
                foreach (var context in _contexts)
                {
                    context.Dispose();
                }

                _contexts.Clear();
            }
            #endregion
        }

        #endregion

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
                _context = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}