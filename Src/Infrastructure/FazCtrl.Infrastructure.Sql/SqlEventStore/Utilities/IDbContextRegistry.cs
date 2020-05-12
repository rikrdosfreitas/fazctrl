using Microsoft.EntityFrameworkCore;

namespace FazCtrl.Infrastructure.Sql.SqlEventStore.Utilities
{
    public interface IDbContextRegistry
    {
        IDbContextManager RegisterContext(DbContext context);
    }
}
