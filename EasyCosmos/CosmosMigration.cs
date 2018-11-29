using System;
using System.Threading.Tasks;
using EasyCosmos.Exceptions;

namespace EasyCosmos
{
    public abstract class CosmosMigration
    {
        protected ICosmosRepository CosmosRepository { get; private set; }

        protected abstract Task Migrate();

        public async Task ExectuteMigration(ICosmosRepository cosmosRepository, int migrationNumber)
        {
            CosmosRepository = cosmosRepository;

            var details = new MigrationDetails();
            details.MigrationNumber = migrationNumber;
            details.id = details.id + 
                         (migrationNumber == -1 ? GetType().Name : migrationNumber.ToString());
            details.Started = DateTime.UtcNow;
            details.Title = GetType().Name;

            await NotifyCosmosOfRunningMigration(details);

            await RunMigration(details);

            await NotifyCosmosOfFinishedMigration(details);
        }

        private async Task NotifyCosmosOfRunningMigration(MigrationDetails details)
        {
            var versionDocument = await CosmosRepository.ReadVersionDocumentAsync();
            if (!(versionDocument is null) && versionDocument.IsApplyingMigration)
                throw new OnGoingMigrationException(
                    $"Could not apply migration: {details.MigrationNumber}: Last migration was not complete");

            await UpdateCosmosMigrationState(details, true, true);
        }

        private async Task RunMigration(MigrationDetails details)
        {
            try
            {
                await Migrate();
                details.DidSucceed = true;
            }
            catch (Exception e)
            {
                details.DidSucceed = false;
                details.FailureMessage = e.Message;
            }
        }

        private async Task NotifyCosmosOfFinishedMigration(MigrationDetails details)
        {
            details.Stopped = DateTime.UtcNow;

            await UpdateCosmosMigrationState(details, !details.DidSucceed, false);
        }

        private async Task UpdateCosmosMigrationState(MigrationDetails details, bool isRunning, bool canFail)
        {
            try
            {
                await CosmosRepository.UpsertDocumentAsync(details);
                await CosmosRepository.UpdateVersionDocumentAsync(details.MigrationNumber, isRunning);
            }
            catch(Exception e)
            {
                if (!canFail) throw e;
            }
            
        }
    }
}