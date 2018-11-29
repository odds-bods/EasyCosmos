using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCosmos.Tests.TestModels;
using Moq;
using Xunit;

namespace EasyCosmos.Tests
{
    public class MigrationRunnerTests : BaseMigrationTests
    {
        private MigrationRunner runner;

        public MigrationRunnerTests()
        {
            this.runner = new MigrationRunner(Mock.Object);
        }
        
        [Fact]
        public async Task CallsMigrationsInOrder()
        {
            var migrations = new TimedTestMigration[]
            {
                new _0003_TimedMigration(), 
                new _0004_TimedMigration()
            };
            
            var runner = new MigrationRunner(Mock.Object);
            await runner.ApplyMigrations(migrations);
            
            Assert.True(migrations[0].ExecutedTime < migrations[1].ExecutedTime,
                "execution times show migration 4 executed before migration 3");
        }      

        [Fact]
        public async Task SuccessfullyExecutesAllMigrationsInAssemblyMigrations()
        {
            var expectedMigrations = GetType().Assembly.FindMigrations()
                .ToDictionaryByMigrationNumber();
            
            var runner = new MigrationRunner(Mock.Object);
            runner.ApplyMigrations(GetType().Assembly);
            
            DidUpdateVersionAndMigrationDocuments();
            Assert.True(updatedVersionDocs.Count == expectedMigrations.Count *2, "should be 2 calls for every migration");
            
            foreach (var migration in expectedMigrations)
            {
                updatedMigrationDocs.Any(kv => kv.Value.MigrationNumber == migration.Key);
            }
        }

        [Fact]
        public void CanUseCustomMigrationCreationMethod()
        {
            var runner = new MigrationRunner(Mock.Object);
            runner.WithCreatMigration(t => new MigrationWithDependency(""));
            runner.ApplyMigrations(new []{typeof(MigrationWithDependency)});
            
            DidUpdateVersionAndMigrationDocuments();
        }

        [Fact]
        public void WithNoMigrationNumberTheTitleIsUsedInIdInstead()
        {
            var expectedTitle = new MigrationDetails().id + nameof(TimedTestMigration);
            
            var migration = new TimedTestMigration();
            runner.ApplyMigrations(new[] {migration});
            DidUpdateVersionAndMigrationDocuments();
            Assert.NotNull(updatedMigrationDocs.First().Value.id);
            Assert.True(updatedMigrationDocs.First().Value.id.Equals(expectedTitle));
        }

        private void DidUpdateVersionAndMigrationDocuments()
        {
            Assert.True(updatedVersionDocs.Count > 0, "failed called to ever update version docs");
            Assert.True(updatedMigrationDocs.Count > 0, "failed called to ever update migration docs");

        }
        
    }
}