using Habanerio.Core.DBs.MongoDB.EFCore;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Microsoft.Extensions.Options;

namespace Tests.Integration.Common;

public class TestDbContext
{
    public class TestAccountsDbContext : AccountsDbContext
    {
        public TestAccountsDbContext(IOptions<MongoDbSettings> options) : base(options) { }
    }

}

public class TestAccountsRepository : MongoDbRepository<AccountDocument>
{
    public TestAccountsRepository(IOptions<MongoDbSettings> options) : base(options) { }
    public TestAccountsRepository(MongoDbContext context) : base(context) { }
}