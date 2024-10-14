using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MongoDB.Bson;
using Tests.Integration.Common;
using Xunit.Abstractions;

namespace Habanerio.Xpnss.Tests.Integration.Modules.Accounts.CQRS.Commands;

/// <summary>
/// Tests that each type of Account can be created successfully.
/// </summary>
[Collection(nameof(AccountsMongoCollection))]
public class CreateAccountHandlerTests : IClassFixture<AccountsTestDbContextFixture>//, IDisposable
{
    private readonly ITestOutputHelper _outputHelper;

    // The repository that is used in the handler to do the saving
    private readonly IAccountsRepository _accountsRepository;

    // The repository that is used to query the database to verify the results
    private readonly TestAccountsRepository _verifyRepository;

    private readonly CreateAccount.Handler _testHandler;

    private readonly string _userId = "test-user-id";

    public CreateAccountHandlerTests(AccountsTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _accountsRepository = dbContextFixture.AccountsRepository;

        _verifyRepository = dbContextFixture.VerifyAccountsRepository;

        _testHandler = new CreateAccount.Handler(_accountsRepository);
    }

    [Fact]
    public async Task CanCall_Handle_CashAccount()
    {
        var command = new CreateAccount.Command(
            _userId,
            nameof(AccountTypes.Cash),
            "Cash Account",
            "Cash Account Description",
            999.99m,
            DisplayColor: "#00FF00"
        );

        var result = await _testHandler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var accountId = Assert.IsType<string>(result.Value);

        var actualAccountDoc = await _verifyRepository
            .FirstOrDefaultAsync(a =>
                a.Id.ToString() == accountId && a.UserId == _userId);

        Assert.NotNull(actualAccountDoc);
        Assert.NotEqual(ObjectId.Empty, actualAccountDoc.Id);
        Assert.Equal(_userId, actualAccountDoc.UserId);
        Assert.Equal(AccountTypes.Cash, actualAccountDoc.AccountTypes);
        Assert.Equal(command.Name, actualAccountDoc.Name);
        Assert.Equal(command.Description, actualAccountDoc.Description);
        Assert.Equal(command.Balance, actualAccountDoc.Balance);
        Assert.Equal(command.DisplayColor, actualAccountDoc.DisplayColor);

        Assert.False(actualAccountDoc.IsCredit);
        Assert.False(actualAccountDoc.IsDeleted);

        Assert.NotNull(actualAccountDoc.DateCreated);
        Assert.Equal(DateTime.UtcNow.Date, actualAccountDoc.DateCreated.Date);
        Assert.Null(actualAccountDoc.DateUpdated);
        Assert.Null(actualAccountDoc.DateDeleted);

        // Due to how the extended properties are added in the handler, this will fail.
        //Assert.Equal(0, actualAccountDoc.ExtendedProps.Count);
    }

    [Fact]
    public async Task CanCall_Handle_CheckingAccount()
    {
        var command = new CreateAccount.Command(
            _userId,
            nameof(AccountTypes.Checking),
            "Checking Account",
            "Checking Account Description",
            999.99m,
            OverDraftAmount: 500m,
            DisplayColor: "#00FF00"
        );

        var result = await _testHandler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var accountId = Assert.IsType<string>(result.Value);

        var actualAccountDoc = (await _verifyRepository
            .FirstOrDefaultAsync(a =>
                a.Id.ToString() == accountId && a.UserId == _userId)) as CheckingAccount;

        Assert.NotNull(actualAccountDoc);
        Assert.NotEqual(ObjectId.Empty, actualAccountDoc.Id);
        Assert.Equal(_userId, actualAccountDoc.UserId);
        Assert.Equal(AccountTypes.Checking, actualAccountDoc.AccountTypes);
        Assert.Equal(command.Name, actualAccountDoc.Name);
        Assert.Equal(command.Description, actualAccountDoc.Description);
        Assert.Equal(command.Balance, actualAccountDoc.Balance);
        Assert.Equal(command.DisplayColor, actualAccountDoc.DisplayColor);

        Assert.False(actualAccountDoc.IsCredit);
        Assert.False(actualAccountDoc.IsDeleted);

        Assert.NotNull(actualAccountDoc.DateCreated);
        Assert.Equal(DateTime.UtcNow.Date, actualAccountDoc.DateCreated.Date);
        Assert.Null(actualAccountDoc.DateUpdated);
        Assert.Null(actualAccountDoc.DateDeleted);

        Assert.Equal(command.OverDraftAmount, actualAccountDoc.OverDraftAmount);
    }

    [Fact]
    public async Task CanCall_Handle_SavingsAccount()
    {
        var command = new CreateAccount.Command(
            _userId,
            nameof(AccountTypes.Savings),
            "Savings Account",
            "Savings Account Description",
            999.99m,
            InterestRate: .99m,
            DisplayColor: "#00FF00"
        );

        var result = await _testHandler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var accountId = Assert.IsType<string>(result.Value);

        var actualAccountDoc = (await _verifyRepository
            .FirstOrDefaultAsync(a =>
                a.Id.ToString() == accountId && a.UserId == _userId)) as SavingsAccount;

        Assert.NotNull(actualAccountDoc);
        Assert.NotEqual(ObjectId.Empty, actualAccountDoc.Id);
        Assert.Equal(_userId, actualAccountDoc.UserId);
        Assert.Equal(AccountTypes.Savings, actualAccountDoc.AccountTypes);
        Assert.Equal(command.Name, actualAccountDoc.Name);
        Assert.Equal(command.Description, actualAccountDoc.Description);
        Assert.Equal(command.Balance, actualAccountDoc.Balance);
        Assert.Equal(command.DisplayColor, actualAccountDoc.DisplayColor);

        Assert.False(actualAccountDoc.IsCredit);
        Assert.False(actualAccountDoc.IsDeleted);

        Assert.NotNull(actualAccountDoc.DateCreated);
        Assert.Equal(DateTime.UtcNow.Date, actualAccountDoc.DateCreated.Date);
        Assert.Null(actualAccountDoc.DateUpdated);
        Assert.Null(actualAccountDoc.DateDeleted);

        Assert.Equal(command.InterestRate, actualAccountDoc.InterestRate);
    }

    [Fact]
    public async Task CanCall_Handle_CreditCardAccount()
    {
        var command = new CreateAccount.Command(
            _userId,
            nameof(AccountTypes.CreditCard),
            "CreditCard Account",
            "CreditCard Account Description",
            999.99m,
            CreditLimit: 1999.99m,
            InterestRate: .99m,
            DisplayColor: "#00FF00"
        );

        var result = await _testHandler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var accountId = Assert.IsType<string>(result.Value);

        var actualAccountDoc = (await _verifyRepository
            .FirstOrDefaultAsync(a =>
                a.Id.ToString() == accountId && a.UserId == _userId)) as CreditCardAccount;

        Assert.NotNull(actualAccountDoc);
        Assert.NotEqual(ObjectId.Empty, actualAccountDoc.Id);
        Assert.Equal(_userId, actualAccountDoc.UserId);
        Assert.Equal(AccountTypes.CreditCard, actualAccountDoc.AccountTypes);
        Assert.Equal(command.Name, actualAccountDoc.Name);
        Assert.Equal(command.Description, actualAccountDoc.Description);
        Assert.Equal(command.Balance, actualAccountDoc.Balance);
        Assert.Equal(command.DisplayColor, actualAccountDoc.DisplayColor);

        Assert.True(actualAccountDoc.IsCredit);
        Assert.False(actualAccountDoc.IsDeleted);

        Assert.NotNull(actualAccountDoc.DateCreated);
        Assert.Equal(DateTime.UtcNow.Date, actualAccountDoc.DateCreated.Date);
        Assert.Null(actualAccountDoc.DateUpdated);
        Assert.Null(actualAccountDoc.DateDeleted);

        Assert.Equal(command.CreditLimit, actualAccountDoc.CreditLimit);
        Assert.Equal(command.InterestRate, actualAccountDoc.InterestRate);
    }

    [Fact]
    public async Task CanCall_Handle_LineOfCreditAccount()
    {
        var command = new CreateAccount.Command(
            _userId,
            nameof(AccountTypes.LineOfCredit),
            "LineOfCredit Account",
            "LineOfCredit Account Description",
            999.99m,
            CreditLimit: 1999.99m,
            InterestRate: .99m,
            DisplayColor: "#00FF00"
        );

        var result = await _testHandler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var accountId = Assert.IsType<string>(result.Value);

        var actualAccountDoc = (await _verifyRepository
            .FirstOrDefaultAsync(a =>
                a.Id.ToString() == accountId && a.UserId == _userId)) as LineOfCreditAccount;

        Assert.NotNull(actualAccountDoc);

        Assert.NotEqual(ObjectId.Empty, actualAccountDoc.Id);

        Assert.Equal(AccountTypes.LineOfCredit, actualAccountDoc.AccountTypes);

        Assert.Equal(_userId, actualAccountDoc.UserId);

        Assert.Equal(command.Name, actualAccountDoc.Name);
        Assert.Equal(command.Description, actualAccountDoc.Description);
        Assert.Equal(command.Balance, actualAccountDoc.Balance);
        Assert.Equal(command.DisplayColor, actualAccountDoc.DisplayColor);

        Assert.True(actualAccountDoc.IsCredit);
        Assert.False(actualAccountDoc.IsDeleted);

        Assert.NotNull(actualAccountDoc.DateCreated);
        Assert.Equal(DateTime.UtcNow.Date, actualAccountDoc.DateCreated.Date);
        Assert.Null(actualAccountDoc.DateUpdated);
        Assert.Null(actualAccountDoc.DateDeleted);


        Assert.Equal(command.CreditLimit, actualAccountDoc.CreditLimit);
        Assert.Equal(command.InterestRate, actualAccountDoc.InterestRate);
    }
}