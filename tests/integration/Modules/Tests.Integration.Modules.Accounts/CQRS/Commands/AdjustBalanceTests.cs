using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MongoDB.Bson;
using Tests.Integration.Common;
using Xunit.Abstractions;

namespace Habanerio.Xpnss.Tests.Integration.Modules.Accounts.CQRS.Commands;

[Collection(nameof(AccountsMongoCollection))]
public class AdjustBalanceTests : IClassFixture<AccountsTestDbContextFixture>//, IDisposable
{
    private readonly ITestOutputHelper _outputHelper;

    // The repository that is used in the handler to do the saving
    private readonly IAccountsRepository _accountsRepository;

    // The repository that is used to query the database to verify the results
    private readonly TestAccountsRepository _verifyRepository;

    private readonly AdjustBalance.Handler _testHandler;

    private readonly string _userId = "1";

    private readonly List<(string accountId, AccountType type)> _availableAccounts;

    public AdjustBalanceTests(AccountsTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _accountsRepository = new AccountsRepository(dbContextFixture.DbContext);

        _verifyRepository = new TestAccountsRepository(dbContextFixture.DbContext);

        _testHandler = new AdjustBalance.Handler(_accountsRepository);

        _availableAccounts = dbContextFixture.AvailableAccounts;
    }

    [Fact]
    public void Can_Instantiate_AdjustBalanceHandler()
    {
        var handler = new AdjustBalance.Handler(_accountsRepository);

        Assert.NotNull(handler);
    }

    [Fact]
    public void CanNot_Instantiate_AdjustBalanceHandler_WithNUll_Repository_ThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => new AdjustBalance.Handler(default));
    }

    [Fact]
    public async Task Can_Adjust_Balance_For_Cash_Account()
    {
        var accountId = _availableAccounts.First(x => x.type == AccountType.Cash).accountId;

        var cashAccount = await _verifyRepository.FirstOrDefaultAsync(a =>
            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

        var previous = cashAccount.Balance;

        var expected = previous + 100;

        var command = new AdjustBalance.Command(
            "1",
            cashAccount.Id.ToString(),
            expected);

        // Act
        var result = await _testHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, cashAccount.Balance);

        //var actualAccount = await _verifyRepository.FirstOrDefaultAsync(a =>
        //    a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

    }
}