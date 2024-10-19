using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Queries;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;

using Tests.Integration.Common;

using Xunit.Abstractions;

namespace Habanerio.Xpnss.Tests.Integration.Modules.Accounts.CQRS.Queries;

/// <summary>
/// Tests that we can call the GetAccount query handler and handle appropriately.
/// </summary>
[Collection(nameof(AccountsMongoCollection))]
public class GetAccountsHandlerTests : IClassFixture<AccountsTestDbContextFixture>//, IDisposable
{
    private readonly ITestOutputHelper _outputHelper;

    // The repository that is used in the handler to do the saving
    private readonly IAccountsRepository _accountsRepository;

    private readonly GetAccounts.Handler _testHandler;

    private readonly string _userId = "test-user-id";

    private readonly List<(string UserId, string AccountId, AccountTypes AccountType)> _availableAccounts;

    public GetAccountsHandlerTests(AccountsTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _accountsRepository = dbContextFixture.AccountsRepository;

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
                case nameof(AccountTypes.Cash):
                    Assert.IsType<CashAccountDto>(accountDto);
                    break;
                case nameof(AccountTypes.Checking):
                    Assert.IsType<CheckingAccountDto>(accountDto);
                    break;
                case nameof(AccountTypes.Savings):
                    Assert.IsType<SavingsAccountDto>(accountDto);
                    break;
                case nameof(AccountTypes.CreditCard):
                    Assert.IsType<CreditCardAccountDto>(accountDto);
                    break;
                case nameof(AccountTypes.LineOfCredit):
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