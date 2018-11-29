namespace EasyCosmos
{
    public class VersionDocument : ITypedDocument
    {
#pragma warning disable IDE1006 // Required to follow naming convention of CosmosDb and the deserialization 
        //It is easier to have this lower case than to add JsonProperty attribute on all properties of the classes that inherit from this one 
        public string id { get; set; } = "_migration_version";
#pragma warning restore IDE1006 //Required to follow naming convention of CosmosDb and the deserialization
        public string Type { get; set; } = nameof(VersionDocument);
        public int LastMigrationApplied { get; set; }
        public bool IsApplyingMigration { get; set; }
    }
}
