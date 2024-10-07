using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MongoDB.Bson;
using Tests.Integration.Common;
using Xunit.Abstractions;

namespace Habanerio.Xpnss.Tests.Integration.Modules.Accounts.CQRS.Commands;

[Collection(nameof(AccountsMongoCollection))]
public class AdjustCreditHandlerTests : IClassFixture<AccountsTestDbContextFixture>//, IDisposable
{
    private readonly ITestOutputHelper _outputHelper;

    // The repository that is used in the handler to do the saving
    private readonly IAccountsRepository _accountsRepository;

    // The repository that is used to query the database to verify the results
    private readonly TestAccountsRepository _verifyRepository;

    private readonly AdjustCreditLimit.Handler _testHandler;

    private readonly string _userId = "1";

    private readonly List<(string accountId, AccountType type)> _availableAccounts;

    public AdjustCreditHandlerTests(AccountsTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _accountsRepository = dbContextFixture.AccountsRepository;
        _verifyRepository = dbContextFixture.VerifyAccountsRepository;

        _testHandler = new AdjustCreditLimit.Handler(dbContextFixture.AccountsRepository);

        _availableAccounts = dbContextFixture.AvailableAccounts;
    }

    [Fact]
    public void Can_Instantiate_AdjustCreditLimitHandler()
    {
        var handler = new AdjustCreditLimit.Handler(_accountsRepository);

        Assert.NotNull(handler);
    }

    [Fact]
    public void CanNot_Instantiate_AdjustCreditLimitHandler_WithNUll_Repository_ThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => new AdjustCreditLimit.Handler(default));
    }

    [Theory]
    [InlineData(AccountType.CreditCard)]
    [InlineData(AccountType.LineOfCredit)]
    public async Task Can_Adjust_CreditLimit(AccountType accountType)
    {
        var accountId = _availableAccounts.First(x => x.type == accountType).accountId;

        var accountDocument = (await _verifyRepository.FirstOrDefaultAsync(a =>
            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId));

        var creditAccount = accountDocument as IHasCreditLimit;

        var previous = creditAccount.CreditLimit;

        var expected = previous + 100;

        var command = new AdjustCreditLimit.Command(
            "1",
            accountDocument.Id.ToString(),
            expected,
            "Updated by `Can_Adjust_CreditLimit`");

        // Act
        var actualResult = await _testHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(actualResult.IsSuccess);

        var actual = actualResult.Value;

        Assert.Equal(expected, actual);

        var actualAccountDocument = await _verifyRepository.FirstOrDefaultAsync(a =>
            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

        Assert.NotNull(actualAccountDocument);

        var actualCreditLimit = (actualAccountDocument as IHasCreditLimit).CreditLimit;

        Assert.Equal(expected, actualCreditLimit);
        Assert.NotEmpty(actualAccountDocument.ChangeHistory);

        var reasons = actualAccountDocument.ChangeHistory.Select(x => x.Reason).ToList();

        Assert.NotEmpty(reasons);
        Assert.Contains("Updated by `Can_Adjust_CreditLimit`", reasons);
    }

    [Theory]
    [InlineData(AccountType.Cash)]
    [InlineData(AccountType.Checking)]
    [InlineData(AccountType.Savings)]
    public async Task Cannot_Adjust_CreditLimit_Invalid_AccountType(AccountType accountType)
    {
        var accountId = _availableAccounts.First(x => x.type == accountType).accountId;

        var accountDocument = await _verifyRepository.FirstOrDefaultAsync(a =>
            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

        var command = new AdjustCreditLimit.Command(
            "1",
            accountDocument.Id.ToString(),
            100000,
            "Updated by `Can_Adjust_CreditLimit`");

        // Act
        var actualResult = await _testHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(actualResult.IsFailed);
        Assert.Equal($"The Account Type `{accountType}` does not support Credit Limits", actualResult.Errors[0].Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CanNotCall_Adjust_CreditLimit_WithEmpty_UserId_IsFailed(string value)
    {
        var result = await _testHandler.Handle(new AdjustCreditLimit.Command(value, "1111111", 1000, ""), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("'User Id' must not be empty.", result.Errors[0].Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CanNotCall_Adjust_CreditLimit_WithEmpty_Account_IsFailed(string value)
    {
        var result = await _testHandler.Handle(new AdjustCreditLimit.Command("1", value, 1000, ""), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("'Account Id' must not be empty.", result.Errors[0].Message);
    }

    [Fact]
    public async Task CanNotCall_Adjust_CreditLimit_WithInvalid_Account_IsFailed()
    {
        var accountId = "sdfgsdf7gsd9fgsdfg";

        var result = await _testHandler.Handle(new AdjustCreditLimit.Command("1", accountId, 1000, ""), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal($"Invalid AccountId: `{accountId}`", result.Errors[0].Message);
    }
}