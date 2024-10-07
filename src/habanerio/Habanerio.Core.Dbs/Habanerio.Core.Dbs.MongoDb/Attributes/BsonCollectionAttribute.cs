namespace Habanerio.Core.Dbs.MongoDb.Attributes;

// https://medium.com/@marekzyla95/mongo-repository-pattern-700986454a0e

/// <summary>
/// Add this above your Mongo Documents
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BsonCollectionAttribute : Attribute
{
    public string CollectionName { get; }

    public BsonCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }
}
