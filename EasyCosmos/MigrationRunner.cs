using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyCosmos
{
    public class MigrationRunner
    {
        private readonly ICosmosRepository client;
        private Func<Type, CosmosMigration> createMigration = 
            t => (CosmosMigration) ObjectActivator.CreateInstance(t);

        public MigrationRunner(
            string database, string collection, string url, string key)
        {
            client = new CosmosRepository(database, collection, url, key);
        }

        public MigrationRunner WithCreatMigration(Func<Type, CosmosMigration> createMigration)
        {
            this.createMigration = createMigration;
            return this;
        }

        public MigrationRunner(ICosmosRepository client)
        {
            this.client = client;
        }

        public async Task ApplyMigrations(Assembly assembly)
        {
            var latestVersion = await client.ReadVersionDocumentAsync();
            
            var migrationTypes = assembly.FindMigrations();
            
            var orderedMigrations = migrationTypes is null
                ? migrationTypes.OrderByNumber()
                : migrationTypes.ToDictionaryByMigrationNumber()
                    .SkipMigrations(latestVersion.LastMigrationApplied)
                    .OrderByNumber();
            
            await ApplyMigrations(orderedMigrations.Select(o=>o.Value));
        }
        
        public async Task ApplyMigrations(IEnumerable<Type> migrationTypes)
        {
            await ApplyMigrations(migrationTypes.Select(t => createMigration(t)));
        }

        public async Task ApplyMigrations(IEnumerable<CosmosMigration> migrations)
        {
            
            foreach (var migration in migrations)
            {
                await migration.ExectuteMigration(client, migration.ReadMigrationNumber());
            }
        }

    }
}
