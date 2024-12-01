using System.ComponentModel.DataAnnotations;
using Habanerio.Core.DBs.MongoDB.EFCore.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Habanerio.Core.DBs.MongoDB.EFCore;

public class MongoDocument : IMongoDocument // DbEntity<ObjectId>, IMongoDbEntity
{
    [BsonId]
    [Key]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Instantiates a new instance of the <see cref="MongoDocument"/> class with a generated Id.
    /// </summary>
    public MongoDocument() : this(ObjectId.GenerateNewId()) { }

    /// <summary>
    /// Instantiates a new instance of the <see cref="MongoDocument"/> class with a provided Id.
    /// </summary>
    /// <param name="id"></param>
    public MongoDocument(ObjectId id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((MongoDocument)obj);
    }

    protected bool Equals(MongoDocument other)
    {
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}