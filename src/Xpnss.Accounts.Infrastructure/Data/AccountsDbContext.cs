using Habanerio.Xpnss.Accounts.Infrastructure.Data.Documents;
using Habanerio.Xpnss.Infrastructure;
using MongoDB.Driver;

namespace Habanerio.Xpnss.Accounts.Infrastructure.Data;

public class AccountsDbContext : XpnssDbContext //<AccountDocument>
{
    public AccountsDbContext(IMongoDatabase mongoDb) : base(mongoDb)
    {
        Configure();
    }

    protected override void Configure()
    {
        // Account Document
        var uniqueUserAccountIdIndex = new CreateIndexModel<AccountDocument>(
            Builders<AccountDocument>.IndexKeys
                .Ascending(a => a.UserId)
                .Ascending(a => a.Id),
            new CreateIndexOptions { Unique = true });

        Collection<AccountDocument>().Indexes.CreateMany(new[] { uniqueUserAccountIdIndex });
    }
}