using Microsoft.EntityFrameworkCore;

namespace FazCtrl.Infrastructure.Sql.SqlEventStore.Transaction
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        {
        }

        public DbSet<TransactionRow> Transactions { get; set; }
    }
}