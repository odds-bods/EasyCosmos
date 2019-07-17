using System;
using System.Threading.Tasks;

namespace EasyCosmos.Tests.TestModels
{
    class TimedTestMigration : CosmosMigration
    {
        public DateTime ExecutedTime { get; private set; }

        protected override Task Migrate()
        {
            ExecutedTime = DateTime.UtcNow;
            return Task.CompletedTask;
        }
        
    }
    
}
