using System.Threading.Tasks;

namespace EasyCosmos.Tests.TestModels
{
    public class MigrationWithDependency : CosmosMigration
    {
        public MigrationWithDependency(string dependency)
        {
            
        }
        
        protected override Task Migrate()
        {
            return Task.CompletedTask;
        }
    }
}