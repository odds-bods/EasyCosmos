namespace EasyCosmos
{
    public interface ITypedDocument
    {
#pragma warning disable IDE1006 // Required to follow naming convention of CosmosDb and the deserialization 
        //It is easier to have this lower case than to add JsonProperty attribute on all properties of the classes that inherit from this one 
        string id { get; set; }
#pragma warning restore IDE1006 //Required to follow naming convention of CosmosDb and the deserialization

        string Type { get; set; }
    }
}
