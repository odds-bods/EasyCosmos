using Microsoft.Azure.Documents;
using System.Threading.Tasks;

namespace EasyCosmos
{
    public interface ICosmosRepository
    {
        IDocumentClient Client { get; }
        Task<T[]> ReadCurrentCosmosDocumentsAsync<T>(string type=null) where T : ITypedDocument;
        Task UpsertDocumentAsync(object document);
        Task<VersionDocument> ReadVersionDocumentAsync();
        Task AddMigrationDetailsAsync(MigrationDetails details);
        Task UpdateVersionDocumentAsync(int latestVersion, bool isRunning);
    }
}
