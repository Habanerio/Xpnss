using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Queries;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;

using MongoDB.Bson;

using Tests.Integration.Common;

using Xunit.Abstractions;

namespace Habanerio.Xpnss.Tests.Integration.Modules.Accounts.CQRS.Queries;

/// <summary>
/// Tests that we can call the GetAccount query handler and handle appropriately.
/// </summary>
[Collection(nameof(AccountsMongoCollection))]
public class GetAccountHandlerTests : IClassFixture<AccountsTestDbContextFixture>//, IDisposable
{
    private readonly ITestOutputHelper _outputHelper;

    // The repository that is used in the handler to do the saving
    private readonly IAccountsRepository _accountsRepository;

    private readonly GetAccount.Handler _testHandler;

    private readonly string _userId = "test-user-id";

    private readonly List<(string UserId, AccountDocument Account)> _availableAccounts;

    public GetAccountHandlerTests(AccountsTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _accountsRepository = dbContextFixture.AccountsRepository;

        _testHandler = new GetAccount.Handler(_accountsRepository);

        _availableAccounts = dbContextFixture.AvailableAccounts;
    }

    [Fact]
    public async Task CanCall_Handle_GetAccount_Cash()
    {
        var accountIds = _availableAccounts
            .Where(x => x.Account.AccountType == AccountTypes.Cash)
            .Select(x => x.Account.Id).ToList();

        foreach (var accountId in accountIds)
        {
            var query = new GetAccount.Query(_userId, accountId.ToString());
            var result = await _testHandler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            var accountDto = Assert.IsType<CashAccountDto>(result.Value);
            Assert.Equal(accountId.ToString(), accountDto.Id);
            Assert.Equal(_userId, accountDto.UserId);
            Assert.Equal(nameof(AccountTypes.Cash), accountDto.AccountType);
            Assert.Contains("Cash Account", accountDto.Name);
        }
    }

    [Fact]
    public async Task CanCall_Handle_GetAccount_Checking()
    {
        var accountIds = _availableAccounts
            .Where(x => x.Account.AccountType == AccountTypes.Checking)
            .Select(x => x.Account.Id).ToList();

        foreach (var accountId in accountIds)
        {
            var query = new GetAccount.Query(_userId, accountId.ToString());
            var result = await _testHandler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            var accountDto = Assert.IsType<CheckingAccountDto>(result.Value);
            Assert.Equal(accountId.ToString(), accountDto.Id);
            Assert.Equal(_userId, accountDto.UserId);
            Assert.Equal(nameof(AccountTypes.Checking), accountDto.AccountType);
            Assert.Contains("Checking Account", accountDto.Name);
        }
    }

    [Fact]
    public async Task CanCall_Handle_GetAccount_Savings()
    {
        var accountIds = _availableAccounts
            .Where(x => x.Account.AccountType == AccountTypes.Savings)
            .Select(x => x.Account.Id).ToList();

        foreach (var accountId in accountIds)
        {
            var query = new GetAccount.Query(_userId, accountId.ToString());
            var result = await _testHandler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            var accountDto = Assert.IsType<SavingsAccountDto>(result.Value);
            Assert.Equal(accountId.ToString(), accountDto.Id);
            Assert.Equal(_userId, accountDto.UserId);
            Assert.Equal(nameof(AccountTypes.Savings), accountDto.AccountType);
            Assert.Contains("Savings Account", accountDto.Name);
        }
    }

    [Fact]
    public async Task CanCall_Handle_GetAccount_CreditCard()
    {
        var accountIds = _availableAccounts
            .Where(x => x.Account.AccountType == AccountTypes.CreditCard)
            .Select(x => x.Account.Id).ToList();

        foreach (var accountId in accountIds)
        {
            var query = new GetAccount.Query(_userId, accountId.ToString());
            var result = await _testHandler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            var accountDto = Assert.IsType<CreditCardAccountDto>(result.Value);
            Assert.Equal(accountId.ToString(), accountDto.Id);
            Assert.Equal(_userId, accountDto.UserId);
            Assert.Equal(nameof(AccountTypes.CreditCard), accountDto.AccountType);
            Assert.Contains("Credit Card Account", accountDto.Name);
        }
    }

    [Fact]
    public async Task CanCall_Handle_GetAccount_LineOfCredit()
    {
        var accountIds = _availableAccounts
            .Where(x => x.Account.AccountType == AccountTypes.LineOfCredit)
            .Select(x => x.Account.Id).ToList();

        foreach (var accountId in accountIds)
        {
            var query = new GetAccount.Query(_userId, accountId.ToString());
            var result = await _testHandler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            var accountDto = Assert.IsType<LineOfCreditAccountDto>(result.Value);
            Assert.Equal(accountId.ToString(), accountDto.Id);
            Assert.Equal(_userId, accountDto.UserId);
            Assert.Equal(nameof(AccountTypes.LineOfCredit), accountDto.AccountType);
            Assert.Contains("Line of Credit Account", accountDto.Name);
        }
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_Handle_GetAccount_EmptyOrNullId(string accountId)
    {
        var query = new GetAccount.Query(_userId, accountId);
        var results = await _testHandler.Handle(query, CancellationToken.None);

        Assert.Equal("'Account Id' must not be empty.", results.Errors[0].Message);
    }

    [Theory]
    [InlineData("dfsgsdfgs dfg sdf")]
    [InlineData("1234567890")]
    [InlineData("1234567890-1234567890-1234567890")]
    [InlineData("000000000000000000000000")]
    public async Task CannotCall_Handle_GetAccount_InvalidId(string accountId)
    {
        var query = new GetAccount.Query(_userId, accountId);
        var results = await _testHandler.Handle(query, CancellationToken.None);

        Assert.True(results.IsFailed);
        Assert.Equal($"Invalid AccountId: `{accountId}`", results.Errors[0].Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_Handle_GetAccount_EmptyOrNullUserId(string userId)
    {
        var query = new GetAccount.Query(userId, ObjectId.GenerateNewId().ToString());
        var results = await _testHandler.Handle(query, CancellationToken.None);

        Assert.True(results.IsFailed);
        Assert.Equal("'User Id' must not be empty.", results.Errors[0].Message);
    }
}