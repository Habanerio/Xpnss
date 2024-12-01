using MongoDB.Driver;

namespace Habanerio.Core.DBs.MongoDB.EFCore.Interfaces;

public interface IMongoDbClient
{
    IMongoDatabase Database { get; }
}
