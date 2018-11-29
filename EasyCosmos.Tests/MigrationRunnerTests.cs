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
            runner = new MigrationRunner(Mock.Object);
        }

        [Fact]
        public async Task SuccessfullExecutesMigrationsInOrder()
        {
            var expectedOrderedIds = GetType().Assembly.FindMigrations()
                .ToDictionaryByMigrationNumber().OrderByNumber()
                .Select(i => i.Key).OrderBy(i => i);

            runner.ApplyMigrations(GetType().Assembly);

            DidUpdateVersionAndMigrationDocuments();

            var runOrder = updatedMigrationDocs.GroupBy(m => m.Value.MigrationNumber).Select(kv => kv.Key);

            Assert.Equal(expectedOrderedIds.Count(), runOrder.Count());
            Assert.True(expectedOrderedIds.SequenceEqual(runOrder));
        }

        [Fact]
        public async Task SuccessfullyExecutesAllMigrationsInAssemblyMigrations()
        {
            var expectedMigrations = GetType().Assembly.FindMigrations()
                .ToDictionaryByMigrationNumber();

            runner.ApplyMigrations(GetType().Assembly);

            DidUpdateVersionAndMigrationDocuments();
            Assert.True(updatedVersionDocs.Count == expectedMigrations.Count * 2,
                "should be 2 calls for every migration");

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
            runner.ApplyMigrations(new[] {typeof(MigrationWithDependency)});

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