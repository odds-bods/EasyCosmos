using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCosmos
{
    public class CosmosRepository : ICosmosRepository
    {
        public IDocumentClient Client { get; private set; }
        private string db;
        private string collection;
        private string collectionLink;

        public CosmosRepository(string database, string collection, string url, string key) : this(database, collection)
        {
            Client = new DocumentClient(new Uri(url), key);
        }

        public CosmosRepository(string database, string collection, IDocumentClient client) : this(database, collection)
        {
            Client = client;
        }

        private CosmosRepository(string database, string collection)
        {
            this.db = database;
            this.collection = collection;
            this.collectionLink = UriFactory.CreateDocumentCollectionUri(database, collection).ToString();
        }

        public async Task CreateCollectionAsync()
        {
            await Client.CreateDatabaseIfNotExistsAsync(new Database { Id = db });
        }

        public async Task CreateDatabaseAsync()
        {
            var databaseLink = UriFactory.CreateDatabaseUri(db).ToString();
            await Client.CreateDocumentCollectionIfNotExistsAsync(
                databaseLink,
                new DocumentCollection { Id = collection }
                );
        }

        public async Task<T[]> ReadCurrentCosmosDocumentsAsync<T>(string type = null) where T : ITypedDocument
        {
            var entityTypeName = type ?? typeof(T).Name;

            var entities = new List<T>();

            try
            {
                var query = Client.CreateDocumentQuery<T>(collectionLink)
                      .Where(e => e.Type == entityTypeName)
                      .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    var response = await query.ExecuteNextAsync<T>();
                    entities.AddRange(response);
                }
            }
            catch
            {
                entities = new List<T>();
            }

            return entities.ToArray();
        }

        public async Task UpsertDocumentAsync(object document)
        {
            await Client.UpsertDocumentAsync(collectionLink, document);
        }

        public async Task<VersionDocument> ReadVersionDocumentAsync()
        {
            var versionDocs = await ReadCurrentCosmosDocumentsAsync<VersionDocument>();
            if (versionDocs is null) return new VersionDocument();

            var doc = versionDocs.FirstOrDefault();
            if (doc is null) return new VersionDocument();

            return doc;
        }

        public async Task AddMigrationDetailsAsync(MigrationDetails details)
        {
            await UpsertDocumentAsync(details);
        }

        public async Task UpdateVersionDocumentAsync(int latestVersion, bool isRunning)
        {
            var currentDocument = await ReadVersionDocumentAsync();
            currentDocument.LastMigrationApplied = latestVersion;
            currentDocument.IsApplyingMigration = isRunning;
            await UpsertDocumentAsync(currentDocument);

        }
    }
}
