using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MongoDB.Bson;
using Tests.Integration.Common;
using Xunit.Abstractions;

namespace Habanerio.Xpnss.Tests.Integration.Modules.Accounts.CQRS.Commands;

[Collection(nameof(AccountsMongoCollection))]
public class AdjustInterestRateHandlerTests : IClassFixture<AccountsTestDbContextFixture>//, IDisposable
{
    private readonly ITestOutputHelper _outputHelper;

    // The repository that is used in the handler to do the saving
    private readonly IAccountsRepository _accountsRepository;

    // The repository that is used to query the database to verify the results
    private readonly TestAccountsRepository _verifyRepository;

    private readonly AdjustInterestRate.Handler _testHandler;

    private readonly string _userId = "test-user-id";

    private readonly List<(string UserId, AccountDocument Account)> _availableAccounts;

    public AdjustInterestRateHandlerTests(AccountsTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _accountsRepository = dbContextFixture.AccountsRepository;
        _verifyRepository = dbContextFixture.VerifyAccountsRepository;

        _testHandler = new AdjustInterestRate.Handler(dbContextFixture.AccountsRepository);

        _availableAccounts = dbContextFixture.AvailableAccounts;
    }

    [Fact]
    public void Can_Instantiate_AdjustInterestRateHandler()
    {
        var handler = new AdjustInterestRate.Handler(_accountsRepository);

        Assert.NotNull(handler);
    }

    [Fact]
    public void CanNot_Instantiate_AdjustInterestRateHandler_With_NUll_Repository_ThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => new AdjustInterestRate.Handler(default));
    }

    [Theory]
    [InlineData(AccountTypes.CreditCard)]
    [InlineData(AccountTypes.LineOfCredit)]
    [InlineData(AccountTypes.Savings)]
    public async Task Can_Adjust_InterestRate(AccountTypes accountTypes)
    {
        var actualAccount = _availableAccounts
            .First(x => x.Account.AccountType == accountTypes);
        var accountId = actualAccount.Account.Id.ToString();

        var accountDocument = await _verifyRepository.FirstOrDefaultAsync(a =>
            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

        var interestAccount = accountDocument as IHasInterestRate;

        var previous = interestAccount.InterestRate;

        var expected = previous + 10;

        var command = new AdjustInterestRate.Command(
            "test-user-id",
            accountDocument.Id.ToString(),
            expected,
            DateTime.Now,
            "Updated by `Can_Adjust_InterestRate`");

        // Act
        var actualResult = await _testHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(actualResult.IsSuccess);

        var actual = actualResult.Value;

        Assert.Equal(expected, actual);

        var actualAccountDocument = await _verifyRepository.FirstOrDefaultAsync(a =>
            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

        Assert.NotNull(actualAccountDocument);

        var actualIHasInterestRateAccount = actualAccountDocument as IHasInterestRate;

        var actualInterestRate = actualIHasInterestRateAccount.InterestRate;

        Assert.Equal(expected, actualInterestRate);
        Assert.NotEmpty(actualAccountDocument.ChangeHistory);

        var reasons = actualAccountDocument.ChangeHistory.Select(x => x.Reason).ToList();

        Assert.NotEmpty(reasons);
        Assert.Contains("Updated by `Can_Adjust_InterestRate`", reasons);
    }

    [Fact]
    public async Task CanNot_Adjust_InterestRate_InterestRate_OverLimit()
    {
        var actualAccount = _availableAccounts
            .First(x => x.Account.AccountType == AccountTypes.CreditCard);
        var accountId = actualAccount.Account.Id.ToString();

        var cashAccountDocument = await _verifyRepository.FirstOrDefaultAsync(a =>
            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

        var expected = 100.1m;

        var command = new AdjustInterestRate.Command(
            "test-user-id",
            cashAccountDocument.Id.ToString(),
            expected,
            DateTime.Now,
            "Updated by `Can_Adjust_InterestRate`");

        // Act
        var actualResult = await _testHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(actualResult.IsFailed);
        Assert.Equal($"'Interest Rate' must be between 0 and 100. You entered {expected}.", actualResult.Errors[0].Message);
    }

    [Fact]
    public async Task CanNot_Adjust_InterestRate_InterestRate_UnderLimit()
    {
        var actualAccount = _availableAccounts
            .First(x => x.Account.AccountType == AccountTypes.CreditCard);
        var accountId = actualAccount.Account.Id.ToString();

        var cashAccountDocument = await _verifyRepository.FirstOrDefaultAsync(a =>
            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

        var expected = -0.01m;

        var command = new AdjustInterestRate.Command(
            "test-user-id",
            cashAccountDocument.Id.ToString(),
            expected,
            DateTime.Now,
            "Updated by `Can_Adjust_InterestRate`");

        // Act
        var actualResult = await _testHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(actualResult.IsFailed);
        Assert.Equal($"'Interest Rate' must be between 0 and 100. You entered {expected}.", actualResult.Errors[0].Message);
    }

    [Theory]
    [InlineData(AccountTypes.Cash)]
    [InlineData(AccountTypes.Checking)]
    public async Task Cannot_Adjust_InterestRate_Invalid_AccountType(AccountTypes accountTypes)
    {
        var actualAccount = _availableAccounts
            .First(x => x.Account.AccountType == accountTypes);
        var accountId = actualAccount.Account.Id.ToString();

        var cashAccountDocument = await _verifyRepository.FirstOrDefaultAsync(a =>
            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

        var previous = cashAccountDocument.Balance;

        var expected = 12;

        var command = new AdjustInterestRate.Command(
            "test-user-id",
            cashAccountDocument.Id.ToString(),
            expected,
            DateTime.Now,
            "Updated by `Can_Adjust_InterestRate`");

        // Act
        var actualResult = await _testHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(actualResult.IsFailed);
        Assert.Equal($"The Account Type `{accountTypes}` does not support Interest Rates", actualResult.Errors[0].Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CanNotCall_Adjust_InterestRate_WithEmpty_UserId_IsFailed(string value)
    {
        var result = await _testHandler.Handle(new AdjustInterestRate.Command(
            value,
            "1111111",
            1000,
            DateTime.Now,
            ""), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("'User Id' must not be empty.", result.Errors[0].Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CanNotCall_Adjust_InterestRate_WithEmpty_Account_IsFailed(string value)
    {
        var result = await _testHandler.Handle(new AdjustInterestRate.Command(
            "test-user-id",
            value,
            1000,
            DateTime.Now,
            ""), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("'Account Id' must not be empty.", result.Errors[0].Message);
    }

    [Fact]
    public async Task CanNotCall_Adjust_InterestRate_WithInvalid_Account_IsFailed()
    {
        var accountId = "sdfgsdf7gsd9fgsdfg";

        var result = await _testHandler.Handle(new AdjustInterestRate.Command(
            "test-user-id",
            accountId,
            10,
            DateTime.Now,
            ""), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal($"Invalid AccountId: `{accountId}`", result.Errors[0].Message);
    }
}