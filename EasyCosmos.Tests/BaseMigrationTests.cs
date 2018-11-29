using System;
using System.Collections.Generic;
using Moq;

namespace EasyCosmos.Tests
{
    public class BaseMigrationTests
    {
        protected Dictionary<DateTime, bool> updatedVersionDocs = new Dictionary<DateTime, bool>();
        protected Dictionary<DateTime,MigrationDetails> updatedMigrationDocs = new Dictionary<DateTime, MigrationDetails>();
        protected Mock<ICosmosRepository> Mock;

        public BaseMigrationTests()
        {
            SetupCosmosRepositoryMock(null, false);
        }
        
        protected void SetupCosmosRepositoryMock(VersionDocument documuent, bool shouldForceVersionDoc)
        {
            new Dictionary<DateTime, bool>();
            new Dictionary<DateTime, MigrationDetails>();
            
            Mock = new CosmosRepositoryBuilder()
                .WithUpsertCallback(e => updatedMigrationDocs.Add(DateTime.UtcNow, e as MigrationDetails))
                .WithReadingVersionDocument(documuent, shouldForceVersionDoc)
                .WithUpdatingVersionDocument((number, isRunning) => updatedVersionDocs.Add(DateTime.UtcNow, isRunning))
                .Mock;
        }
    }
}