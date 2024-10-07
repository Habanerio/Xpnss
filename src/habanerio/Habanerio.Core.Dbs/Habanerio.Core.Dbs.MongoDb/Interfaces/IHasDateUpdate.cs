namespace Habanerio.Core.Dbs.MongoDb.Interfaces;

public interface IHasDateUpdated
{
    DateTime DateUpdated { get; set; }
}

public interface IHasDateTimeOffsetUpdated
{
    DateTimeOffset DateUpdated { get; set; }
}