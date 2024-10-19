using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
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
        var accountDto = Assert.IsType<CashAccountDto>(result.Value);

        Assert.NotNull(accountDto);
        Assert.NotEqual(ObjectId.Empty.ToString(), accountDto.Id);
        Assert.Equal(_userId, accountDto.UserId);
        Assert.Equal(AccountTypes.Cash.ToString(), accountDto.AccountType);
        Assert.Equal(command.Name, accountDto.Name);
        Assert.Equal(command.Description, accountDto.Description);
        Assert.Equal(command.Balance, accountDto.Balance);
        Assert.Equal(command.DisplayColor, accountDto.DisplayColor);

        Assert.False(accountDto.IsCredit);
        Assert.False(accountDto.IsDeleted);

        Assert.NotNull(accountDto.DateCreated);
        Assert.Equal(DateTime.UtcNow.Date, accountDto.DateCreated.Date);
        Assert.Null(accountDto.DateUpdated);
        Assert.Null(accountDto.DateDeleted);

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
        var accountDto = Assert.IsType<CheckingAccountDto>(result.Value);

        Assert.NotNull(accountDto);
        Assert.NotEqual(ObjectId.Empty.ToString(), accountDto.Id);
        Assert.Equal(_userId, accountDto.UserId);
        Assert.Equal(AccountTypes.Checking.ToString(), accountDto.AccountType);
        Assert.Equal(command.Name, accountDto.Name);
        Assert.Equal(command.Description, accountDto.Description);
        Assert.Equal(command.Balance, accountDto.Balance);
        Assert.Equal(command.DisplayColor, accountDto.DisplayColor);

        Assert.False(accountDto.IsCredit);
        Assert.False(accountDto.IsDeleted);

        Assert.NotNull(accountDto.DateCreated);
        Assert.Equal(DateTime.UtcNow.Date, accountDto.DateCreated.Date);
        Assert.Null(accountDto.DateUpdated);
        Assert.Null(accountDto.DateDeleted);

        Assert.Equal(command.OverDraftAmount, accountDto.OverDraftAmount);
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
        var accountDto = Assert.IsType<SavingsAccountDto>(result.Value);

        Assert.NotNull(accountDto);
        Assert.NotEqual(ObjectId.Empty.ToString(), accountDto.Id);
        Assert.Equal(_userId, accountDto.UserId);
        Assert.Equal(AccountTypes.Savings.ToString(), accountDto.AccountType);
        Assert.Equal(command.Name, accountDto.Name);
        Assert.Equal(command.Description, accountDto.Description);
        Assert.Equal(command.Balance, accountDto.Balance);
        Assert.Equal(command.DisplayColor, accountDto.DisplayColor);

        Assert.False(accountDto.IsCredit);
        Assert.False(accountDto.IsDeleted);

        Assert.NotNull(accountDto.DateCreated);
        Assert.Equal(DateTime.UtcNow.Date, accountDto.DateCreated.Date);
        Assert.Null(accountDto.DateUpdated);
        Assert.Null(accountDto.DateDeleted);

        Assert.Equal(command.InterestRate, accountDto.InterestRate);
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
        var accountDto = Assert.IsType<CreditCardAccountDto>(result.Value);

        Assert.NotNull(accountDto);
        Assert.NotEqual(ObjectId.Empty.ToString(), accountDto.Id);
        Assert.Equal(_userId, accountDto.UserId);
        Assert.Equal(AccountTypes.CreditCard.ToString(), accountDto.AccountType);
        Assert.Equal(command.Name, accountDto.Name);
        Assert.Equal(command.Description, accountDto.Description);
        Assert.Equal(command.Balance, accountDto.Balance);
        Assert.Equal(command.DisplayColor, accountDto.DisplayColor);

        Assert.True(accountDto.IsCredit);
        Assert.False(accountDto.IsDeleted);

        Assert.NotNull(accountDto.DateCreated);
        Assert.Equal(DateTime.UtcNow.Date, accountDto.DateCreated.Date);
        Assert.Null(accountDto.DateUpdated);
        Assert.Null(accountDto.DateDeleted);

        Assert.Equal(command.CreditLimit, accountDto.CreditLimit);
        Assert.Equal(command.InterestRate, accountDto.InterestRate);
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
        var accountDto = Assert.IsType<LineOfCreditAccountDto>(result.Value);

        Assert.NotNull(accountDto);

        Assert.NotEqual(ObjectId.Empty.ToString(), accountDto.Id);

        Assert.Equal(AccountTypes.LineOfCredit.ToString(), accountDto.AccountType);

        Assert.Equal(_userId, accountDto.UserId);

        Assert.Equal(command.Name, accountDto.Name);
        Assert.Equal(command.Description, accountDto.Description);
        Assert.Equal(command.Balance, accountDto.Balance);
        Assert.Equal(command.DisplayColor, accountDto.DisplayColor);

        Assert.True(accountDto.IsCredit);
        Assert.False(accountDto.IsDeleted);

        Assert.NotNull(accountDto.DateCreated);
        Assert.Equal(DateTime.UtcNow.Date, accountDto.DateCreated.Date);
        Assert.Null(accountDto.DateUpdated);
        Assert.Null(accountDto.DateDeleted);


        Assert.Equal(command.CreditLimit, accountDto.CreditLimit);
        Assert.Equal(command.InterestRate, accountDto.InterestRate);
    }
}