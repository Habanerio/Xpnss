//using Habanerio.Xpnss.Modules.Accounts.Common;
//using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
//using Habanerio.Xpnss.Modules.Accounts.Interfaces;
//using MongoDB.Bson;
//using Tests.Integration.Common;
//using Xunit.Abstractions;

//namespace Habanerio.Xpnss.Tests.Integration.Modules.Accounts.CQRS.Commands;

//[Collection(nameof(AccountsMongoCollection))]
//public class AdjustOverdraftAmountHandlerTests : IClassFixture<AccountsTestDbContextFixture>//, IDisposable
//{
//    private readonly ITestOutputHelper _outputHelper;

//    // The repository that is used in the handler to do the saving
//    private readonly IAccountsRepository _accountsRepository;

//    // The repository that is used to query the database to verify the results
//    private readonly TestAccountsRepository _verifyRepository;

//    private readonly AdjustOverdraftAmount.Handler _testHandler;

//    private readonly string _userId = "test-user-id";

//    private readonly List<(string accountId, AccountTypes type)> _availableAccounts;

//    public AdjustOverdraftAmountHandlerTests(AccountsTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
//    {
//        _outputHelper = outputHelper;

//        _accountsRepository = dbContextFixture.AccountsRepository;
//        _verifyRepository = dbContextFixture.VerifyAccountsRepository;

//        _testHandler = new AdjustOverdraftAmount.Handler(dbContextFixture.AccountsRepository);

//        _availableAccounts = dbContextFixture.AvailableAccounts;
//    }

//    [Fact]
//    public void Can_Instantiate_AdjustOverdraftAmountHandler()
//    {
//        var handler = new AdjustOverdraftAmount.Handler(_accountsRepository);

//        Assert.NotNull(handler);
//    }

//    [Fact]
//    public void CanNot_Instantiate_AdjustOverdraftAmountHandler_WithNUll_Repository_ThrowException()
//    {
//        Assert.Throws<ArgumentNullException>(() => new AdjustOverdraftAmount.Handler(default));
//    }

//    [Theory]
//    [InlineData(AccountTypes.CreditCard)]
//    [InlineData(AccountTypes.LineOfCredit)]
//    public async Task Can_Adjust_InterestRate(AccountTypes accountTypes)
//    {
//        var accountId = _availableAccounts.First(x => x.type == accountTypes).accountId;

//        var cashAccountDocument = await _verifyRepository.FirstOrDefaultAsync(a =>
//            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

//        var previous = (decimal)cashAccountDocument.ExtendedProps.Find(p => p.Key.Equals("InterestRate")).Value;

//        var expected = previous + 100;

//        var command = new AdjustOverdraftAmount.Command(
//            "test-user-id",
//            cashAccountDocument.Id.ToString(),
//            expected,
//            "Updated by `Can_Adjust_InterestRate`");

//        // Act
//        var actualResult = await _testHandler.Handle(command, CancellationToken.None);

//        // Assert
//        Assert.True(actualResult.IsSuccess);

//        var actual = actualResult.Value;

//        Assert.Equal(expected, actual);

//        var actualAccountDocument = await _verifyRepository.FirstOrDefaultAsync(a =>
//            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

//        Assert.NotNull(actualAccountDocument);

//        var actualInterestRate = (decimal)actualAccountDocument.ExtendedProps.Find(p => p.Key.Equals("InterestRate")).Value;

//        Assert.Equal(expected, actualInterestRate);
//        Assert.NotEmpty(actualAccountDocument.ChangeHistory);

//        var reasons = actualAccountDocument.ChangeHistory.Select(x => x.Reason).ToList();

//        Assert.NotEmpty(reasons);
//        Assert.Contains("Updated by `Can_Adjust_InterestRate`", reasons);
//    }

//    [Theory]
//    [InlineData(AccountTypes.Cash)]
//    [InlineData(AccountTypes.Checking)]
//    [InlineData(AccountTypes.Savings)]
//    public async Task Cannot_Adjust_InterestRate_Invalid_AccountType(AccountTypes accountTypes)
//    {
//        var accountId = _availableAccounts.First(x => x.type == accountTypes).accountId;

//        var cashAccountDocument = await _verifyRepository.FirstOrDefaultAsync(a =>
//            a.Id == ObjectId.Parse(accountId) && a.UserId == _userId);

//        var previous = cashAccountDocument.Balance;

//        var expected = previous + 100;

//        var command = new AdjustOverdraftAmount.Command(
//            "test-user-id",
//            cashAccountDocument.Id.ToString(),
//            expected,
//            "Updated by `Can_Adjust_InterestRate`");

//        // Act
//        var actualResult = await _testHandler.Handle(command, CancellationToken.None);

//        // Assert
//        Assert.True(actualResult.IsFailed);
//        Assert.Equal($"The Account Type `{accountTypes}` does not support InterestRate Limits", actualResult.Errors[0].Message);
//    }

//    [Theory]
//    [InlineData("")]
//    [InlineData(" ")]
//    [InlineData(null)]
//    public async Task CanNotCall_Adjust_InterestRate_WithEmpty_UserId_IsFailed(string value)
//    {
//        var result = await _testHandler.Handle(new AdjustOverdraftAmount.Command(value, "1111111", 1000, ""), CancellationToken.None);

//        Assert.True(result.IsFailed);
//        Assert.Equal("'User Id' must not be empty.", result.Errors[0].Message);
//    }

//    [Theory]
//    [InlineData("")]
//    [InlineData(" ")]
//    [InlineData(null)]
//    public async Task CanNotCall_Adjust_InterestRate_WithEmpty_Account_IsFailed(string value)
//    {
//        var result = await _testHandler.Handle(new AdjustOverdraftAmount.Command("test-user-id", value, 1000, ""), CancellationToken.None);

//        Assert.True(result.IsFailed);
//        Assert.Equal("'Account Id' must not be empty.", result.Errors[0].Message);
//    }

//    [Fact]
//    public async Task CanNotCall_Adjust_InterestRate_WithInvalid_Account_IsFailed()
//    {
//        var accountId = "sdfgsdf7gsd9fgsdfg";

//        var result = await _testHandler.Handle(new AdjustOverdraftAmount.Command("test-user-id", accountId, 1000, ""), CancellationToken.None);

//        Assert.True(result.IsFailed);
//        Assert.Equal($"Invalid AccountId: `{accountId}`", result.Errors[0].Message);
//    }
//}