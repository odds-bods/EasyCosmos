using Moq;
using System;
using System.Threading.Tasks;

namespace EasyCosmos.Tests
{
    class CosmosRepositoryBuilder
    {
        public Mock<ICosmosRepository> Mock;

        public CosmosRepositoryBuilder()
        {
            Mock = new Mock<ICosmosRepository>();
        }

        public CosmosRepositoryBuilder WithUpsertCallback(Action<object> callback)
        {
            Mock.Setup(repository => repository.UpsertDocumentAsync(It.IsAny<object>())).Callback(callback)
                .Returns(Task.CompletedTask)
                .Verifiable();

            return this;
        }

        public CosmosRepositoryBuilder WithReadingVersionDocument(
            VersionDocument document = null, bool forceNull = false)
        {
            var defaultDoc = forceNull
                ? null
                : new VersionDocument
                {
                    IsApplyingMigration = false,
                    LastMigrationApplied = 0
                };
            
            Mock.Setup(m => m.ReadVersionDocumentAsync())
                .Returns(Task.FromResult(document ??  defaultDoc));

            return this;
        }

        public CosmosRepositoryBuilder WithUpdatingVersionDocument(Action<int, bool> callback)
        {
            Mock.Setup(m => m.UpdateVersionDocumentAsync(It.IsAny<int>(), It.IsAny<bool>()))
                .Callback(callback)
                .Returns(Task.CompletedTask);

            return this;
        }
    }
}
