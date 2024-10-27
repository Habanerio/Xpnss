using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Interfaces;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Habanerio.Xpnss.Modules.Categories.Data;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using Habanerio.Xpnss.Modules.Transactions.Common;
using Habanerio.Xpnss.Modules.Transactions.Data;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Testcontainers.MongoDb;

namespace Tests.Integration.Common;

/// <summary>
/// Fixture for the TestDb Context using the TestDb Container
/// </summary>
public class TestDbContainerFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container = new MongoDbBuilder()
        .WithImage("mongo:6.0")
        .Build();

    public string ConnectionString => _container.GetConnectionString();
    public string ContainerId => $"{_container.Id}";

    public virtual async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public virtual Task DisposeAsync()
        => _container.DisposeAsync().AsTask();
}

[CollectionDefinition(nameof(AccountsMongoCollection))]
public class AccountsMongoCollection : ICollectionFixture<AccountsTestDbContextFixture> { }

public class AccountsTestDbContextFixture : TestDbContainerFixture
{
    private IMongoDbContext<AccountDocument>? _dbContext;

    public IMongoDbContext<AccountDocument> DbContext => _dbContext;

    public IAccountsRepository AccountsRepository { get; set; }

    public TestAccountsRepository VerifyAccountsRepository { get; set; }

    public List<(string UserId, AccountDocument Account)> AvailableAccounts = [];

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var options = Options.Create(new MongoDbSettings
        {
            ConnectionString = ConnectionString,
            DatabaseName = "Xpnss-Test",
            EnableDetailedErrors = true,
            EnableSensitiveDataLogging = true
        });

        _dbContext = new TestDbContext.TestAccountsDbContext(options);

        AccountsRepository = new AccountsRepository(options);
        VerifyAccountsRepository = new TestAccountsRepository(options);

        await PopulateData();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }

    //// Not able to do this in the TestDbContext
    private async Task PopulateData()
    {
        // If the db already exists, delete it and reseed it
        var userId = "test-user-id";

        var accountDocs = new List<AccountDocument>
        {
            CashAccount.New(userId,
                "Test Cash Account",
                "Test Cash Account 1 - Description",
                100,
                "#f0f0f0"),

            CheckingAccount.New(userId,
                "Test Checking Account",
                "Test Checking Account 1 - Description",
                100,
                500,
                "#f0f0f0"),
            SavingsAccount.New(userId,
                "Test Savings Account",
                "Test Savings Account 1 - Description",
                100,
                0.03m,
                "#f0f0f0"),
            CreditCardAccount.New(userId,
                "Test Credit Card Account",
                "Test Credit Card Account 1 - Description",
                100,
                1000,
                0.15m,
                "#f0f0f0"),
            LineOfCreditAccount.New(userId,
                "Test Line of Credit Account",
                "Test Line of Credit Account 1 - Description",
                100,
                1000,
                0.12m,
                "#f0f0f0"),

        };

        foreach (var accountDoc in accountDocs)
        {
            var result = await AccountsRepository.AddAsync(accountDoc);

            if (result.IsSuccess)
                AvailableAccounts.Add((accountDoc.UserId, result.Value));
        }
    }
}


[CollectionDefinition(nameof(CategoriesMongoCollection))]
public class CategoriesMongoCollection : ICollectionFixture<CategoriesTestDbContextFixture> { }

public class CategoriesTestDbContextFixture : TestDbContainerFixture
{
    private IMongoDbContext<CategoryDocument>? _dbContext;

    public IMongoDbContext<CategoryDocument> DbContext => _dbContext;

    public ICategoriesRepository CategoriesRepository { get; set; }

    public TestCategoriesRepository VerifyCategoriesRepository { get; set; }

    public List<(string UserId, CategoryDocument Category)> ActualCategories = [];

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var options = Options.Create(new MongoDbSettings
        {
            ConnectionString = ConnectionString,
            DatabaseName = "Xpnss-Test",
            EnableDetailedErrors = true,
            EnableSensitiveDataLogging = true
        });

        _dbContext = new TestDbContext.TestCategoriesDbContext(options);

        CategoriesRepository = new CategoriesRepository(options);
        VerifyCategoriesRepository = new TestCategoriesRepository(options);

        await PopulateData();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }

    private async Task PopulateData()
    {
        var categoryDocs = new List<CategoryDocument>
        {
            CategoryDocument.New("test-user-id", "Test Category 6", "Test Category 6 Description",6),
            CategoryDocument.New("test-user-id", "Test Category 1", "Test Category 1 Description", 1),
            CategoryDocument.New("test-user-id", "Test Category 5", "Test Category 5 Description",5),
            CategoryDocument.New("test-user-id", "Test Category 3", "Test Category 3 Description",3),
            CategoryDocument.New("test-user-id", "Test Category 2", "Test Category 2 Description",2),
            CategoryDocument.New("test-user-id", "Test Category 4", "Test Category 4 Description",4),
        };

        categoryDocs[0].AddSubCategory(
            "Test Category 6 - SubCategory 1",
            "Test Category 6 - SubCategory 1 Description",
            1);
        categoryDocs[0].AddSubCategory(
            "Test Category 6 - SubCategory 3",
            "Test Category 6 - SubCategory 3 Description",
            3);
        categoryDocs[0].AddSubCategory(
            "Test Category 6 - SubCategory 2",
            "Test Category 6 - SubCategory 2 Description",
            2);

        foreach (var categoryDoc in categoryDocs)
        {
            var result = await CategoriesRepository.AddAsync(categoryDoc);

            if (result.IsSuccess)
                ActualCategories.Add((categoryDoc.UserId, result.Value));
        }
    }
}


[CollectionDefinition(nameof(TransactionsMongoCollection))]
public class TransactionsMongoCollection : ICollectionFixture<TransactionsTestDbContextFixture> { }

public class TransactionsTestDbContextFixture : TestDbContainerFixture
{
    private IMongoDbContext<TransactionDocument>? _dbContext;

    public IMongoDbContext<TransactionDocument> DbContext => _dbContext;

    public ITransactionsRepository TransactionsRepository { get; set; }

    public TestTransactionsRepository VerifyTransactionsRepository { get; set; }

    public List<(string UserId, TransactionDocument Transaction)> ActualTransactions = [];

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var options = Options.Create(new MongoDbSettings
        {
            ConnectionString = ConnectionString,
            DatabaseName = "Xpnss-Test",
            EnableDetailedErrors = true,
            EnableSensitiveDataLogging = true
        });

        _dbContext = new TestDbContext.TestTransactionsDbContext(options);

        TransactionsRepository = new TransactionsRepository(options);
        VerifyTransactionsRepository = new TestTransactionsRepository(options);

        await PopulateData();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }

    //// Not able to do this in the TestDbContext
    private async Task PopulateData()
    {
        var categoryIds = new List<string>
        {
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
        };

        var accountIds = new List<string>
        {
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
        };

        var transactionDocs = new List<TransactionDocument>
        {
            TransactionDocument.New(
                "test-user-id",
                accountIds[0],
                DateTime.UtcNow.AddDays(-365),
                new List<TransactionItem>
                {
                    TransactionItem.New(100, "Test Transaction 1", categoryIds[0])
                },
                TransactionTypes.Keys.PURCHASE,
                "Test Transaction 1",
                TransactionMerchant.New("Test Merchant 1", "Test Merchant Location 1")),
            TransactionDocument.New(
                "test-user-id",
                accountIds[0],
                DateTime.UtcNow.AddDays(-200),
                new List<TransactionItem>
                {
                    TransactionItem.New(200, "Test Transaction 2", categoryIds[1])
                },
                TransactionTypes.Keys.PURCHASE,
                "Test Transaction 2",
                TransactionMerchant.New("Test Merchant 2", "Test Merchant Location 2")),
            TransactionDocument.New(
                "test-user-id-2",
                accountIds[1],
                DateTime.UtcNow.AddDays(-100),
                new List<TransactionItem>
                {
                    TransactionItem.New(300, "Test Transaction 3", categoryIds[0])
                },
                TransactionTypes.Keys.PURCHASE,
                "Test Transaction 3",
                TransactionMerchant.New("Test Merchant 3", "Test Merchant Location 3")),
            TransactionDocument.New(
                "test-user-id",
                accountIds[3],
                DateTime.UtcNow.AddDays(-50),
                new List<TransactionItem>
                {
                    TransactionItem.New(400, "Test Transaction 4", categoryIds[1])
                },
                TransactionTypes.Keys.PURCHASE,
                "Test Transaction 4",
                TransactionMerchant.New("Test Merchant 4", "Test Merchant Location 4")),
            TransactionDocument.New(
                "test-user-id-2",
                accountIds[2],
                DateTime.UtcNow.AddDays(-20),
                new List<TransactionItem>
                {
                    TransactionItem.New(500, "Test Transaction 5", categoryIds[0])
                },
                TransactionTypes.Keys.PURCHASE,
                "Test Transaction 5",
                TransactionMerchant.New("Test Merchant 5", "Test Merchant Location 5")),
            TransactionDocument.New(
                "test-user-id",
                accountIds[0],
                DateTime.UtcNow.AddDays(-10),
                new List<TransactionItem>
                {
                    TransactionItem.New(600, "Test Transaction 6", categoryIds[1])
                },
                TransactionTypes.Keys.PURCHASE,
                "Test Transaction 6",
                TransactionMerchant.New("Test Merchant 6", "Test Merchant Location 6")),
        };

        foreach (var transactionDoc in transactionDocs)
        {
            var result = await TransactionsRepository.AddAsync(transactionDoc);

            if (result.IsSuccess)
                ActualTransactions.Add((transactionDoc.UserId, result.Value));
        }
    }
}