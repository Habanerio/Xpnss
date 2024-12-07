using AutoFixture;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.BankAccounts;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Tests.Unit.Domain.Accounts;

public class CheckingAccountTests : TestsBase
{
    private readonly CheckingAccount _testClass;

    public CheckingAccountTests()
    {
        _testClass = CheckingAccount.Load(
            AccountId.New,
            UserId.New,
            new AccountName("Checking Account"),
            new Money(2000),
            AutoFixture.Create<string>(),
            null,
            AutoFixture.Create<string>(),
            "#110022",
            AutoFixture.Create<string>(),
            AutoFixture.Create<bool>(),
            new Money(1000),
            AutoFixture.Create<int>(),
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
        var closedDate = AutoFixture.Create<DateTime?>();
        var description = AutoFixture.Create<string>();
        var displayColor = "#110022";
        var extAcctId = AutoFixture.Create<string>();
        var institutionName = AutoFixture.Create<string>();
        var isDefault = AutoFixture.Create<bool>();
        var overDraftAmount = NewMoney(2134);
        var sortOrder = AutoFixture.Create<int>();
        var dateCreated = AutoFixture.Create<DateTime>();
        var dateClosed = AutoFixture.Create<DateTime?>();
        var dateDeleted = AutoFixture.Create<DateTime?>();
        var dateUpdated = AutoFixture.Create<DateTime?>();

        // Act
        var result = CheckingAccount.Load(
            id,
            userId,
            accountName,
            balance,
            institutionName,
            closedDate,
            description,
            displayColor,
            extAcctId,
            true,
            overDraftAmount,
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
        Assert.Equal(overDraftAmount, result.OverdraftLimit);
        Assert.Equal(dateCreated, result.DateCreated);
        Assert.Equal(dateClosed, result.ClosedDate);
        Assert.Equal(dateDeleted, result.DateDeleted);
        Assert.Equal(dateUpdated, result.DateUpdated);
    }

    [Fact]
    public void CannotCall_Load_WithNull_Id()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CheckingAccount.Load(
                null,
                NewUserId(),
                NewAccountName(),
                NewMoney(4564),
                AutoFixture.Create<string>(),
                null,
                AutoFixture.Create<string>(),
                "#110022",
                AutoFixture.Create<string>(),
                AutoFixture.Create<bool>(),
                NewMoney(58756),
                AutoFixture.Create<int>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));
    }

    [Fact]
    public void CannotCall_Load_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CheckingAccount.Load(
                NewAccountId(),
                null,
                NewAccountName(),
                NewMoney(4564),
                AutoFixture.Create<string>(),
                null,
                AutoFixture.Create<string>(),
                "#110022",
                AutoFixture.Create<string>(),
                AutoFixture.Create<bool>(),
                NewMoney(58756),
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
        var overDraftAmount = NewMoney(4564);

        // Act
        var result = CheckingAccount.New(userId, accountName, description, displayColor, overDraftAmount);

        // Assert
        Assert.True(!string.IsNullOrWhiteSpace(result.Id.Value));
        Assert.Equal(userId, result.UserId);
        Assert.Equal(accountName, result.Name);
        Assert.Equal(0, result.Balance.Value);
        Assert.Equal(description, result.Description);
        Assert.Equal(displayColor, result.DisplayColor);
        Assert.Equal(overDraftAmount, result.OverdraftLimit);

        Assert.False(result.IsCredit);
    }

    [Fact]
    public void CannotCall_New_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CheckingAccount.New(
                null,
                NewAccountName(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<string>(),
                NewMoney(45645)));
    }


    /// <summary>
    /// Tests that for Checking Accounts, a new DEPOSIT transaction will increase the Balance.
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
    /// Tests that for Checking Accounts, a new PURCHASE transaction will decrease the Balance.
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
    /// Tests that for Checking Accounts, a previous DEPOSIT transaction will decrease the Balance.
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
    /// Tests that for Checking Accounts, a previous PURCHASE transaction will increase the Balance.
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

    /// <summary>
    /// Cannot perform action when Account is deleted
    /// </summary>
    [Fact]
    public void CannotCall_ApplyTransactionAmount_DEPOSIT_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<InvalidOperationException>(() =>
            _testClass.AddTransactionAmount(new Money(1), TransactionEnums.TransactionKeys.DEPOSIT));
    }

    /// <summary>
    /// Cannot perform action when Account is deleted
    /// </summary>
    [Fact]
    public void CannotCall_ApplyTransactionAmount_PURCHASE_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<InvalidOperationException>(() =>
            _testClass.AddTransactionAmount(new Money(1), TransactionEnums.TransactionKeys.PURCHASE));
    }


    /// <summary>
    /// Cannot perform action when Account is deleted
    /// </summary>
    [Fact]
    public void CannotCall_UndoTransactionAmount_PURCHASE_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<InvalidOperationException>(() =>
            _testClass.RemoveTransactionAmount(new Money(1), TransactionEnums.TransactionKeys.PURCHASE));
    }

    /// <summary>
    /// Cannot perform action when Account is deleted
    /// </summary>
    [Fact]
    public void CannotCall_UndoTransactionAmount_DEPOSIT_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<InvalidOperationException>(() =>
            _testClass.RemoveTransactionAmount(new Money(1),
                TransactionEnums.TransactionKeys.DEPOSIT));
    }

    [Fact]
    public void CanCall_Close()
    {
        var dateClosed = DateTime.Now.AddDays(-1);

        // Act
        _testClass.Close(dateClosed);

        // Assert
        Assert.NotNull(_testClass.ClosedDate);
        Assert.Equal(dateClosed, _testClass.ClosedDate);

        Assert.True(_testClass.IsClosed);

        Assert.NotNull(_testClass.DateUpdated);
        Assert.Equal(DateTime.Now.ToUniversalTime(),
            _testClass.DateUpdated.Value, new TimeSpan(0, 0, 0, 10));
    }

    [Fact]
    public void CanCall_ReOpen()
    {
        var dateClosed = DateTime.Now.AddDays(-1);

        // Act
        _testClass.Close(dateClosed);

        Assert.True(_testClass.IsClosed);

        // Act
        _testClass.Open();

        // Assert
        Assert.Null(_testClass.ClosedDate);
        Assert.False(_testClass.IsClosed);
        Assert.Equal(DateTime.Now.ToUniversalTime(),
            _testClass.DateUpdated.Value, new TimeSpan(0, 0, 0, 10));
    }

    [Fact]
    public void CannotCall_Close_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<InvalidOperationException>(() => _testClass.Close(DateTime.Now));
    }

    [Fact]
    public void CanCall_Delete()
    {
        _testClass.Delete();

        Assert.NotNull(_testClass.DateDeleted);
        Assert.Equal(DateTime.Now.ToUniversalTime(),
            _testClass.DateDeleted.Value, new TimeSpan(0, 0, 0, 10));

        Assert.True(_testClass.IsDeleted);

        Assert.NotNull(_testClass.DateUpdated);
        Assert.Equal(DateTime.Now.ToUniversalTime(),
            _testClass.DateUpdated.Value, new TimeSpan(0, 0, 0, 10));
    }
}