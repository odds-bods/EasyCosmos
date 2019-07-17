using System;

namespace EasyCosmos
{
    public class MigrationDetails : ITypedDocument
    {
#pragma warning disable IDE1006 // Required to follow naming convention of CosmosDb and the deserialization 
        //It is easier to have this lower case than to add JsonProperty attribute on all properties of the classes that inherit from this one 
        public string id { get; set; } = "_migration_";
#pragma warning restore IDE1006 //Required to follow naming convention of CosmosDb and the deserialization
        public string Type { get; set; } = nameof(MigrationDetails);
        public bool DidSucceed { get; set; }
        public DateTime Started { get; set; }
        public DateTime Stopped { get; set; }
        public int MigrationNumber { get; set; }
        public string FailureMessage { get; set; }
        public string Title { get; set; }
    }
}
