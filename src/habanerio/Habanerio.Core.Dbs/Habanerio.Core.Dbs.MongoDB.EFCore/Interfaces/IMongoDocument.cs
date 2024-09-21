using Habanerio.Core.DBs.EFCore.Interfaces;
using MongoDB.Bson;

namespace Habanerio.Core.DBs.MongoDB.EFCore.Interfaces;

/// <summary>
/// A wrapper interface for MongoDB entities that specified that the Id should be of type ObjectId.
/// </summary>
public interface IMongoDocument : IDbEntity<ObjectId> { }