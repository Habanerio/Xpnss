using Habanerio.Core.Dbs.MongoDb;
using Habanerio.Core.Dbs.MongoDb.Interfaces;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Microsoft.Extensions.Options;
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

    public List<(string AccountId, AccountType AccountType)> AvailableAccounts = [];

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

        var accountDtos = new List<AccountDto>()
        {
            new CashAccountDto(
                id: "66c1ee4ac96ab13b745d65da",
                userId: "1",
                name: "Test Cash Account 1",
                description: "Test Cash Account 1 - Description",
                balance: 100m,
                displayColor: "#ff0000",
                dateCreated: DateTimeOffset.UtcNow.AddDays(-365)),
            new CheckingAccountDto(
                id: "66c1ee4ac96ab13b745d65ea",
                userId: "1",
                name: "Test Checking Account 1",
                description: "Test Checking Account 1 - Description",
                balance: 100m,
                overDraftAmount: 500m,
                displayColor: "#ff0000",
                dateCreated: DateTimeOffset.UtcNow.AddDays(-365)),
            new SavingsAccountDto(
                id: "66c1ee4ac96ab13b745d65eb",
                userId: "1",
                name: "Deleted Savings Account 2",
                description: "Deleted Savings Account 2 - Description",
                balance: 200m,
                interestRate: 0.03m,
                displayColor: "#00ff00",
                dateCreated: DateTimeOffset.UtcNow.AddDays(-200),
                dateDeleted: DateTimeOffset.UtcNow.AddDays(-20)),
            new SavingsAccountDto(
                id: "66c1ee4ac96ab13b745d65ec",
                userId: "1",
                name: "Test Savings Account 3",
                description: "Test Savings Account 3 - Description",
                balance: 300m,
                interestRate: 0.03m,
                displayColor: "#00ff00",
                dateCreated: DateTimeOffset.UtcNow.AddDays(-50)),
            new CreditCardAccountDto(
                id: "66c1ee4ac96ab13b745d65ee",
                userId: "1",
                name: "Test Credit Card Account 1",
                description: "Test Credit Card Account 1 - Description",
                balance: 500m,
                creditLimit: 1000m,
                interestRate: 0.15m,
                displayColor: "#00ffff", dateCreated: DateTimeOffset.UtcNow.AddDays(-100)),

            new LineOfCreditAccountDto(
                id: "66c1ee4ac96ab13b745d65ef",
                userId: "1",
                name: "Test Line of Credit Account 1",
                description: "Test Line of Credit Account 1 - Description",
                balance: 700m,
                creditLimit: 1000m,
                displayColor: "#ff00ff",
                interestRate: 0.12m,
                dateCreated: DateTimeOffset.UtcNow.AddDays(-150)),
        };

        var accountDocuments = Habanerio.Xpnss.Modules.Accounts.Mappers.DtoToDocumentMappings.Map(accountDtos);

        foreach (var accountDocument in accountDocuments)
        {
            var result = await AccountsRepository.AddAsync(accountDocument);

            if (result.IsSuccess)
                AvailableAccounts.Add((result.Value.ToString(), accountDocument.AccountType));
        }

        //Thread.Sleep(500);
    }
}
