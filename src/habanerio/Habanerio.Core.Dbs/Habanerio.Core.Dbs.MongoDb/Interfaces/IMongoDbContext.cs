using MongoDB.Driver;

namespace Habanerio.Core.Dbs.MongoDb.Interfaces;

public interface IMongoDbContext<TDocument> where TDocument : IMongoDocument
{
    IMongoCollection<TDocument> Collection();
}