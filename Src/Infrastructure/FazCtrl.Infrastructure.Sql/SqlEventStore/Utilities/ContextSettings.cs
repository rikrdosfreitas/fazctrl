namespace FazCtrl.Infrastructure.Sql.SqlEventStore.Utilities
{
    public class ContextSettings
    {
        public ContextSettings(string schema, string migrationAssembly)
        {
            Schema = schema;
            MigrationAssembly = migrationAssembly;
        }

        public string Schema { get; private set; }
        public string MigrationAssembly { get; private set; }
    }
}