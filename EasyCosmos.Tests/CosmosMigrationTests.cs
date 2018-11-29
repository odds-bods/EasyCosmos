using System;
using System.Threading.Tasks;
using Xunit;
using EasyCosmos.Tests.TestModels;
using System.Collections.Generic;
using System.Linq;
using EasyCosmos.Exceptions;
using Moq;

namespace EasyCosmos.Tests
{
    public class CosmosMigrationTests : BaseMigrationTests
    {   
        [Fact]
        public async Task DoesSendDetailsBeforeRunningMigration()
        {
            var migration = new TimedTestMigration();
            
            await migration.ExectuteMigration(Mock.Object, 1);

            Assert.True(updatedMigrationDocs.Count == 2, "expected 2 updates");
            Assert.True(updatedMigrationDocs.First().Key < migration.ExecutedTime,
                "the migration was started before the noted start time in details");
        }

        [Fact]
        public async Task DoesSendMigrationTitleInDetails()
        {
            var migration = new TimedTestMigration();

            await migration.ExectuteMigration(Mock.Object, migration.ReadMigrationNumber());
            
            Assert.True(updatedMigrationDocs.First().Value.Title.Equals(nameof(TimedTestMigration)));
        }

        [Fact]
        public async Task DoesUpdateMigrationVersionDocumentBeforeMigrationRun()
        {
            var migration = new TimedTestMigration();
            await migration.ExectuteMigration(Mock.Object, 1);

            Assert.True(updatedVersionDocs.Count == 2);
            Assert.True(updatedVersionDocs.First().Key < migration.ExecutedTime,
                "the migration document was called after the migration started");
            Assert.True(updatedVersionDocs.First().Value, "the version document was updated for is running to be false");
        }

        [Fact]
        public async Task DoesUpdateVersionDocumentWhenMigrationFailed()
        {
            var updatedVersionDocs = new Dictionary<DateTime, bool>();
            var updatedMigrationDocs = new List<MigrationDetails>();

            var migration = new FailureMigration(true);
            var mockRepo = new CosmosRepositoryBuilder()
                .WithUpsertCallback(e => updatedMigrationDocs.Add(e as MigrationDetails))
                .WithReadingVersionDocument()
                .WithUpdatingVersionDocument((number, isRunning) => updatedVersionDocs.Add(DateTime.UtcNow, isRunning))
                .Mock;

            await migration.ExectuteMigration(mockRepo.Object, 1);

            Assert.True(updatedVersionDocs.Count == 2);
            Assert.True(updatedVersionDocs.Last().Value,
                "the version document was updated for is running to be false");
            Assert.True(updatedMigrationDocs.Count == 2);
            Assert.False(updatedMigrationDocs.Last().DidSucceed, "expected did succeed to be set to false");
            Assert.NotNull(updatedMigrationDocs.Last().FailureMessage);
        }

        [Fact]
        public async Task DoesNotRunMigrationIfOneAlreadyRunning()
        {
            var updatedVersionDocs = new Dictionary<DateTime, bool>();
            var updatedMigrationDocs = new List<MigrationDetails>();

            var migration = new _0001_TestMigrationFirst();
            var mockRepo = new CosmosRepositoryBuilder()
                .WithUpsertCallback(e => updatedMigrationDocs.Add(e as MigrationDetails))
                .WithReadingVersionDocument(new VersionDocument() {IsApplyingMigration = true})
                .WithUpdatingVersionDocument((number, isRunning) => updatedVersionDocs.Add(DateTime.UtcNow, isRunning))
                .Mock;

            Assert.ThrowsAsync<OnGoingMigrationException>(
                () => migration.ExectuteMigration(mockRepo.Object, 1));
            Assert.True(updatedVersionDocs.Count == 0);
            Assert.True(updatedMigrationDocs.Count == 0);
        }

        [Fact]
        public async Task DoesRunMigrationIfFailToGetVersionDocument()
        {
            var updatedVersionDocs = new Dictionary<DateTime, bool>();
            var updatedMigrationDocs = new List<MigrationDetails>();

            var migration = new _0001_TestMigrationFirst();
            var mockRepo = new CosmosRepositoryBuilder()
                .WithUpsertCallback(e => updatedMigrationDocs.Add(e as MigrationDetails))
                .WithReadingVersionDocument(null, true)
                .WithUpdatingVersionDocument((number, isRunning) => updatedVersionDocs.Add(DateTime.UtcNow, isRunning))
                .Mock;

            migration.ExectuteMigration(mockRepo.Object, 1);

            Assert.True(updatedVersionDocs.Count > 0, "called to update version docs");
            Assert.True(updatedMigrationDocs.Count > 0, "called to update migration docs");
        }
        
        [Fact]
        public async Task MigrationCanBeUsedToSetupDatabase()
        {
            var callTimes = 0;
            Mock.Setup(x => x.UpdateVersionDocumentAsync(It.IsAny<int>(), It.IsAny<bool>()))
                .Callback(() => callTimes++).Returns(
                callTimes < 1 ? Task.CompletedTask : throw new Exception());
            
            var migration = new _0003_TimedMigration();

            migration.ExectuteMigration(Mock.Object, 1);
           
            Assert.Equal(updatedMigrationDocs.Count, 2);
        }

        [Fact]
        public async Task ExceptionThrownIfFinishUpdateVersionDocumentFailure()
        {
            Mock.Setup(x => x.UpdateVersionDocumentAsync(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Exception()));
            
            var migration = new _0003_TimedMigration();

            Assert.ThrowsAnyAsync<Exception>(() => migration.ExectuteMigration(Mock.Object, 1));
        }
        
        [Fact]
        public async Task ExceptionThrownIfSecondUpdateDocumentFailure()
        {
            Mock.Setup(x => x.UpsertDocumentAsync(It.IsAny<object>()))
                .Returns(Task.FromResult(new Exception()));
            
            var migration = new _0003_TimedMigration();

            Assert.ThrowsAnyAsync<Exception>(() => migration.ExectuteMigration(Mock.Object, 1));
        }
        
        [Fact]
        public async Task ExceptionThrownIfSecondUpdateDocumentFailureAndMigrationFails()
        {
            Mock.Setup(x => x.UpsertDocumentAsync(It.IsAny<object>()))
                .Returns(Task.FromResult(new Exception()));
            
            var migration = new FailureMigration(true);

            Assert.ThrowsAnyAsync<Exception>(() => migration.ExectuteMigration(Mock.Object, 1));
        }
    }
}