using AutoFixture;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CashAccounts;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Tests.Unit.Domain.Accounts;

public class CashAccountTests : TestsBase
{
    private readonly CashAccount _testClass;

    public CashAccountTests()
    {
        _testClass = CashAccount.Load(
            AccountId.New,
            UserId.New,
            new AccountName("Test Account"),
            NewMoney(1000),
            "Test Description",
            "#110022",
            true,
            1,
            DateTime.UtcNow,
            null,
            null);
    }

    [Fact]
    public void CanCall_Load()
    {
        // Arrange
        var id = NewAccountId();
        var userId = NewUserId();
        var accountName = NewAccountName();
        var balance = NewMoney(3453);
        var description = AutoFixture.Create<string>();
        var displayColor = "#223344";
        var isDefault = AutoFixture.Create<bool>();
        var sortOrder = AutoFixture.Create<int>();
        var dateCreated = AutoFixture.Create<DateTime>();
        var dateDeleted = AutoFixture.Create<DateTime?>();
        var dateUpdated = AutoFixture.Create<DateTime?>();

        // Act
        var result = CashAccount.Load(
            id,
            userId,
            accountName,
            balance,
            description,
            displayColor,
            isDefault,
            sortOrder,
            dateCreated,
            dateUpdated,
            dateDeleted);

        // Assert
        Assert.Equal(id, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(accountName, result.Name);
        Assert.Equal(balance, result.Balance);
        Assert.Equal(description, result.Description);
        Assert.Equal(displayColor, result.DisplayColor);
        Assert.Equal(isDefault, result.IsDefault);
        Assert.Equal(sortOrder, result.SortOrder);
        Assert.Equal(dateCreated, result.DateCreated);
        Assert.Equal(dateDeleted, result.DateDeleted);
        Assert.Equal(dateUpdated, result.DateUpdated);
    }

    [Fact]
    public void CannotCall_Load_WithNull_Id()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CashAccount.Load(
                null,
                NewUserId(),
                NewAccountName(),
                NewMoney(4564),
                AutoFixture.Create<string>(),
                "#110022",
                AutoFixture.Create<bool>(),
                AutoFixture.Create<int>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));
    }

    [Fact]
    public void CannotCall_Load_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CashAccount.Load(
                NewAccountId(),
                null,
                NewAccountName(),
                NewMoney(4564),
                AutoFixture.Create<string>(),
                "#110022",
                AutoFixture.Create<bool>(),
                AutoFixture.Create<int>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));
    }

    [Fact]
    public void CanCall_New()
    {
        // Arrange
        var userId = NewUserId();
        var accountName = NewAccountName();
        var description = AutoFixture.Create<string>();
        var displayColor = "#C8C8C8";

        // Act
        var result = CashAccount.New(userId, accountName, description, displayColor);

        // Assert
        Assert.True(!string.IsNullOrWhiteSpace(result.Id.Value));
        Assert.Equal(userId, result.UserId);
        Assert.Equal(accountName, result.Name);
        Assert.Equal(0, result.Balance.Value);
        Assert.Equal(description, result.Description);
        Assert.Equal(displayColor, result.DisplayColor);

        Assert.False(result.IsCredit);
    }

    [Fact]
    public void CannotCall_New_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CashAccount.New(
                default(UserId),
                NewAccountName(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<string>()));
    }


    /// <summary>
    /// Tests that for Cash Accounts, a new DEPOSIT transaction will increase the Balance.
    /// </summary>
    [Fact]
    public void CanCall_ApplyTransactionAmount_DEPOSIT()
    {
        var value = NewMoney(1000);

        var previousValue = _testClass.Balance;

        _testClass.AddTransactionAmount(value, TransactionEnums.TransactionKeys.DEPOSIT);

        // Assert
        Assert.Equal(previousValue + value, _testClass.Balance);
    }

    /// <summary>
    /// Tests that for Cash Accounts, a new PURCHASE transaction will decrease the Balance.
    /// </summary>
    [Fact]
    public void CanCall_ApplyTransactionAmount_PURCHASE()
    {
        var value = NewMoney(1000);

        var previousValue = _testClass.Balance;

        _testClass.AddTransactionAmount(value, TransactionEnums.TransactionKeys.PURCHASE);

        // Assert
        Assert.Equal(previousValue - value, _testClass.Balance);
    }


    /// <summary>
    /// Tests that for Cash Accounts, a previous DEPOSIT transaction will decrease the Balance.
    /// </summary>
    [Fact]
    public void CanCall_UndoTransactionAmount_DEPOSIT()
    {
        var value = NewMoney(1000);

        var previousValue = _testClass.Balance;

        _testClass.RemoveTransactionAmount(value, TransactionEnums.TransactionKeys.DEPOSIT);

        // Assert
        Assert.Equal(previousValue - value, _testClass.Balance);
    }

    /// <summary>
    /// Tests that for Cash Accounts, a previous PURCHASE transaction will increase the Balance.
    /// </summary>
    [Fact]
    public void CanCall_UndoTransactionAmount_PURCHASE()
    {
        var value = NewMoney(1000);

        var previousValue = _testClass.Balance;

        _testClass.RemoveTransactionAmount(value, TransactionEnums.TransactionKeys.PURCHASE);

        // Assert
        Assert.Equal(previousValue + value, _testClass.Balance);
    }


    /// <summary>
    /// Cannot perform action when creditLimit is negative
    /// </summary>
    [Fact]
    public void CannotCall_ApplyTransactionAmount_WithNegativeAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _testClass.AddTransactionAmount(NewMoney(-1), TransactionEnums.TransactionKeys.DEPOSIT));
    }

    /// <summary>
    /// Cannot perform action when creditLimit is negative
    /// </summary>
    [Fact]
    public void CannotCall_UndoTransactionAmount_WithNegativeAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _testClass.RemoveTransactionAmount(NewMoney(-1), TransactionEnums.TransactionKeys.DEPOSIT));
    }
}