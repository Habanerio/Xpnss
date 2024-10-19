using Habanerio.Core.Dbs.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Modules.Accounts.Data;

public class AccountsDbContext : MongoDbContext<AccountDocument>
{
    public AccountsDbContext(IOptions<MongoDbSettings> options) : base(options)
    {
        CreateIndex();
    }

    private void CreateIndex()
    {
        var indexKeysDefinition = Builders<AccountDocument>.IndexKeys
            .Ascending(a => a.UserId)
            .Ascending(a => a.Id);

        var createIndexModel = new CreateIndexModel<AccountDocument>(
            indexKeysDefinition,
            new CreateIndexOptions { Name = "UserId_Id_Index" }
        );

        try
        {
            Collection().Indexes.CreateOne(createIndexModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}