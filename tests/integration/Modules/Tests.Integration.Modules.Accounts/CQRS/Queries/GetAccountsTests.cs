using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Queries;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Tests.Integration.Plutus;
using Xunit.Abstractions;

namespace Habanerio.Xpnss.Tests.Integration.Modules.Accounts.CQRS.Queries;

/// <summary>
/// Tests that we can call the GetAccount query handler and handle appropriately.
/// </summary>
[Collection(nameof(AccountsMongoCollection))]
public class GetAccountsTests : IClassFixture<AccountsTestDbContextFixture>//, IDisposable
{
    private readonly ITestOutputHelper _outputHelper;

    // The repository that is used in the handler to do the saving
    private readonly IAccountsRepository _accountsRepository;

    private readonly GetAccounts.Handler _testHandler;

    private readonly string _userId = "1";

    private readonly List<(string AccountId, AccountType AccountType)> _availableAccounts;

    public GetAccountsTests(AccountsTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _accountsRepository = new AccountsRepository(dbContextFixture.DbContext);

        _testHandler = new GetAccounts.Handler(_accountsRepository);

        _availableAccounts = dbContextFixture.AvailableAccounts;
    }

    [Fact]
    public async Task CanCall_Handle_GetAccounts()
    {
        var query = new GetAccounts.Query(_userId);
        var results = await _testHandler.Handle(query, CancellationToken.None);

        Assert.True(results.IsSuccess);
        Assert.NotNull(results.Value);

        var accountDtos = results.Value;

        foreach (var accountDto in accountDtos)
        {
            switch (accountDto.AccountType)
            {
                case AccountType.Cash:
                    Assert.IsType<CashAccountDto>(accountDto);
                    break;
                case AccountType.Checking:
                    Assert.IsType<CheckingAccountDto>(accountDto);
                    break;
                case AccountType.Savings:
                    Assert.IsType<SavingsAccountDto>(accountDto);
                    break;
                case AccountType.CreditCard:
                    Assert.IsType<CreditCardAccountDto>(accountDto);
                    break;
                case AccountType.LineOfCredit:
                    Assert.IsType<LineOfCreditAccountDto>(accountDto);
                    break;
                default:
                    Assert.Fail("The dto's type does not match its AccountType");
                    break;
            }
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_Handle_GetAccounts_InvalidUserId(string userId)
    {
        var query = new GetAccounts.Query(userId);
        var results = await _testHandler.Handle(query, CancellationToken.None);

        Assert.True(results.IsFailed);
        Assert.Equal("'User Id' must not be empty.", results.Errors[0].Message);
    }
}