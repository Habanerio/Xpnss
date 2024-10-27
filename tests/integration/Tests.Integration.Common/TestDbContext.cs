using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Categories.Data;
using Habanerio.Xpnss.Modules.Transactions.Data;
using Microsoft.Extensions.Options;

namespace Tests.Integration.Common;

public class TestDbContext
{
    public class TestAccountsDbContext : AccountsDbContext
    {
        public TestAccountsDbContext(IOptions<MongoDbSettings> options) : base(options) { }
    }

    public class TestCategoriesDbContext : CategoriesDbContext
    {
        public TestCategoriesDbContext(IOptions<MongoDbSettings> options) : base(options) { }
    }


    public class TestTransactionsDbContext : TransactionsDbContext
    {
        public TestTransactionsDbContext(IOptions<MongoDbSettings> options) : base(options) { }
    }
}

public class TestAccountsRepository : MongoDbRepository<AccountDocument>
{
    public TestAccountsRepository(IOptions<MongoDbSettings> options) : base(options) { }

    public TestAccountsRepository(string connectionString, string databaseName) : base(connectionString, databaseName) { }
}

public class TestCategoriesRepository : MongoDbRepository<CategoryDocument>
{
    public TestCategoriesRepository(IOptions<MongoDbSettings> options) : base(options) { }

    public TestCategoriesRepository(string connectionString, string databaseName) : base(connectionString, databaseName) { }
}

public class TestTransactionsRepository : MongoDbRepository<TransactionDocument>
{
    public TestTransactionsRepository(IOptions<MongoDbSettings> options) : base(options) { }

    public TestTransactionsRepository(string connectionString, string databaseName) : base(connectionString, databaseName) { }
}