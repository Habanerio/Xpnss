namespace Habanerio.Core.DBs.MongoDB.EFCore;
public class MongoDbSettings : DbSettings
{
    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    /// <value>
    /// The name of the database.
    /// </value>
    public virtual string DatabaseName { get; set; } = "";

    public virtual bool EnableSensitiveDataLogging { get; set; } = false;

    public virtual bool EnableDetailedErrors { get; set; } = false;
}
