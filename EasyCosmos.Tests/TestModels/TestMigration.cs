using System;
using System.Threading.Tasks;

namespace EasyCosmos.Tests.TestModels
{
    
    class _0007__TestMigrationSeven : CosmosMigration
    {
        protected override Task Migrate()
        {
            return Task.CompletedTask;
        }
    }

    class _0008__TestMigrationEight : CosmosMigration
    {
        protected override Task Migrate()
        {
            return Task.CompletedTask;
        }
    }

    class _0001_TestMigrationFirst : CosmosMigration
    {
        protected override Task Migrate()
        {
            return Task.CompletedTask;       
        }
    }

    class _0002_TestMigrationSecond : CosmosMigration
    {
        protected override Task Migrate()
        {
            return Task.CompletedTask;
        }
    }

    class _0003_TimedMigration : TimedTestMigration
    {
        
    }
    
    class _0004_TimedMigration : TimedTestMigration
    {
        
    }
}
