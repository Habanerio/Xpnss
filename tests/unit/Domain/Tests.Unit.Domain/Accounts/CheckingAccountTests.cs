using AutoFixture;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Tests.Unit.Domain.Accounts;

public class CheckingAccountTests : BaseAccountTests
{
    private readonly CheckingAccount _testClass;

    public CheckingAccountTests()
    {
        _testClass = AutoFixture.Create<CheckingAccount>();
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
        var displayColor = "#110022";
        var overDraftAmount = NewMoney(2134);
        var dateCreated = AutoFixture.Create<DateTime>();
        var dateClosed = AutoFixture.Create<DateTime?>();
        var dateDeleted = AutoFixture.Create<DateTime?>();
        var dateUpdated = AutoFixture.Create<DateTime?>();

        // Act
        var result = CheckingAccount.Load(id, userId, accountName, balance, description, displayColor, overDraftAmount, dateCreated, dateClosed, dateDeleted, dateUpdated);

        // Assert
        Assert.Equal(id, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(accountName, result.Name);
        Assert.Equal(balance, result.Balance);
        Assert.Equal(description, result.Description);
        Assert.Equal(displayColor, result.DisplayColor);
        Assert.Equal(overDraftAmount, result.OverdraftAmount);
        Assert.Equal(dateCreated, result.DateCreated);
        Assert.Equal(dateClosed, result.DateClosed);
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
                "#110022",
                NewMoney(58756),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
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
                "#110022",
                NewMoney(58756),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
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
        Assert.Equal(overDraftAmount, result.OverdraftAmount);
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

        _testClass.ApplyTransactionAmount(value, TransactionTypes.Keys.DEPOSIT);

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

        _testClass.ApplyTransactionAmount(value, TransactionTypes.Keys.PURCHASE);

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

        _testClass.UndoTransactionAmount(value, TransactionTypes.Keys.DEPOSIT);

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

        _testClass.UndoTransactionAmount(value, TransactionTypes.Keys.PURCHASE);

        // Assert
        Assert.Equal(previousValue + value, _testClass.Balance);
    }


    /// <summary>
    /// Cannot perform action when creditLimit is negative
    /// </summary>
    [Fact]
    public void CannotCall_ApplyTransactionAmount_WithNegativeAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _testClass.ApplyTransactionAmount(NewMoney(-1), TransactionTypes.Keys.DEPOSIT));
    }

    /// <summary>
    /// Cannot perform action when creditLimit is negative
    /// </summary>
    [Fact]
    public void CannotCall_UndoTransactionAmount_WithNegativeAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _testClass.UndoTransactionAmount(NewMoney(-1), TransactionTypes.Keys.DEPOSIT));
    }

    /// <summary>
    /// Cannot perform action when Account is deleted
    /// </summary>
    [Fact]
    public void CannotCall_ApplyTransactionAmount_DEPOSIT_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<InvalidOperationException>(() => _testClass.ApplyTransactionAmount(new Money(1), TransactionTypes.Keys.DEPOSIT));
    }

    /// <summary>
    /// Cannot perform action when Account is deleted
    /// </summary>
    [Fact]
    public void CannotCall_ApplyTransactionAmount_PURCHASE_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<InvalidOperationException>(() => _testClass.ApplyTransactionAmount(new Money(1), TransactionTypes.Keys.PURCHASE));
    }


    /// <summary>
    /// Cannot perform action when Account is deleted
    /// </summary>
    [Fact]
    public void CannotCall_UndoTransactionAmount_PURCHASE_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<InvalidOperationException>(() => _testClass.UndoTransactionAmount(new Money(1), TransactionTypes.Keys.PURCHASE));
    }

    /// <summary>
    /// Cannot perform action when Account is deleted
    /// </summary>
    [Fact]
    public void CannotCall_UndoTransactionAmount_DEPOSIT_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<InvalidOperationException>(() => _testClass.UndoTransactionAmount(new Money(1), TransactionTypes.Keys.DEPOSIT));
    }


    /// <summary>
    /// Cannot perform action when Account is deleted
    /// </summary>
    [Fact]
    public void CanCall_UpdateBalance()
    {
        // Arrange
        var previousValue = _testClass.Balance;

        var value = NewMoney(1000);

        // Act
        _testClass.UpdateBalance(value);

        // Assert
        Assert.Equal(previousValue + value, _testClass.Balance);
    }

    /// <summary>
    /// Cannot perform action when Account is deleted
    /// </summary>
    [Fact]
    public void CannotCall_UpdatedBalance_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<InvalidOperationException>(() => _testClass.UpdateBalance(new Money(1)));
    }

    [Fact]
    public void CanCall_Close()
    {
        var dateClosed = DateTime.Now.AddDays(-1);

        // Act
        _testClass.Close(dateClosed);

        // Assert
        Assert.NotNull(_testClass.DateClosed);
        Assert.Equal(dateClosed, _testClass.DateClosed);

        Assert.True(_testClass.IsClosed);

        Assert.NotNull(_testClass.DateUpdated);
        Assert.Equal(DateTime.Now.ToUniversalTime(), _testClass.DateUpdated.Value, new TimeSpan(0, 0, 0, 10));
    }

    [Fact]
    public void CanCall_ReOpen()
    {
        var dateClosed = DateTime.Now.AddDays(-1);

        // Act
        _testClass.Close(dateClosed);

        Assert.True(_testClass.IsClosed);

        // Act
        _testClass.ReOpen();

        // Assert
        Assert.Null(_testClass.DateClosed);
        Assert.False(_testClass.IsClosed);
        Assert.Equal(DateTime.Now.ToUniversalTime(), _testClass.DateUpdated.Value, new TimeSpan(0, 0, 0, 10));
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
        Assert.Equal(DateTime.Now.ToUniversalTime(), _testClass.DateDeleted.Value, new TimeSpan(0, 0, 0, 10));

        Assert.True(_testClass.IsDeleted);

        Assert.NotNull(_testClass.DateUpdated);
        Assert.Equal(DateTime.Now.ToUniversalTime(), _testClass.DateUpdated.Value, new TimeSpan(0, 0, 0, 10));
    }
}