using AutoFixture;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.BankAccounts;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Tests.Unit.Domain.Accounts;

public class CreditLineAccountTests : TestsBase
{
    private readonly CreditLineAccount _testClass;

    public CreditLineAccountTests()
    {
        var userId = NewUserId();
        var accountName = NewAccountName();
        var description = AutoFixture.Create<string>();
        var displayColor = "#C8C8C8";
        var creditLimit = NewMoney(4564);
        var interestRate = NewPercentageRate(23);

        _testClass = CreditLineAccount.New(userId, accountName, description, displayColor, creditLimit, interestRate);
    }

    [Fact]
    public void CanCall_Load()
    {
        // Arrange
        var id = NewAccountId();
        var userId = NewUserId();
        var accountName = NewAccountName();
        var balance = NewMoney(3453);
        var bankName = AutoFixture.Create<string>();
        DateTime? closedDate = AutoFixture.Create<bool>() ? DateTime.UtcNow.AddDays(-12) : default;
        var creditLimit = NewMoney(2134);
        var description = AutoFixture.Create<string>();
        var displayColor = "#110022";
        var extAcctId = AutoFixture.Create<string>();

        var isDefault = AutoFixture.Create<bool>();
        var sortOrder = AutoFixture.Create<int>();
        var dateCreated = AutoFixture.Create<DateTime>();
        var dateDeleted = AutoFixture.Create<DateTime?>();
        var dateUpdated = AutoFixture.Create<DateTime?>();

        var interestRate = NewPercentageRate(34);

        // Act
        var result = CreditLineAccount.Load(
            id,
            userId,
            accountName,
            balance,
            bankName,
            closedDate,
            description,
            displayColor,
            extAcctId,
            creditLimit,
            interestRate,
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
        Assert.Equal(closedDate, result.ClosedDate);
        Assert.Equal(creditLimit, result.CreditLimit);
        Assert.Equal(description, result.Description);
        Assert.Equal(displayColor, result.DisplayColor);
        Assert.Equal(extAcctId, result.ExtAcctId);
        Assert.Equal(bankName, result.BankName);
        Assert.Equal(interestRate, result.InterestRate);

        Assert.True(result.IsCredit);
        Assert.True(result.IsClosed == closedDate is not null);
        Assert.True(result.IsDeleted == dateDeleted is not null);

        Assert.Equal(dateCreated, result.DateCreated);
        Assert.Equal(dateDeleted, result.DateDeleted);
        Assert.Equal(dateUpdated, result.DateUpdated);
        Assert.Equal(interestRate, result.InterestRate);
    }

    [Fact]
    public void CannotCall_Load_WithNull_Id()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CreditLineAccount.Load(
                null,
                NewUserId(),
                NewAccountName(),
                NewMoney(4564),
                AutoFixture.Create<string>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<string>(),
                "#110022",
                AutoFixture.Create<string>(),
                NewMoney(58756),
                NewPercentageRate(34),
                false,
                AutoFixture.Create<int>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>()));
    }

    [Fact]
    public void CannotCall_Load_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CreditLineAccount.Load(
                NewAccountId(),
                null,
                NewAccountName(),
                NewMoney(4564),
                AutoFixture.Create<string>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<string>(),
                "#110022",
                AutoFixture.Create<string>(),
                NewMoney(58756),
                NewPercentageRate(34),
                false,
                AutoFixture.Create<int>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime>(),
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
        var creditLimit = NewMoney(4564);
        var interestRate = NewPercentageRate(23);

        // Act
        var result = CreditLineAccount.New(userId, accountName, description, displayColor, creditLimit, interestRate);

        // Assert
        Assert.True(!string.IsNullOrWhiteSpace(result.Id.Value));
        Assert.Equal(userId, result.UserId);
        Assert.Equal(accountName, result.Name);
        Assert.Equal(0, result.Balance.Value);
        Assert.Equal(creditLimit, result.CreditLimit);
        Assert.Equal(description, result.Description);
        Assert.Equal(displayColor, result.DisplayColor);
        Assert.Equal(interestRate, result.InterestRate);

        Assert.True(result.IsCredit);
    }

    [Fact]
    public void CannotCall_New_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CreditLineAccount.New(
                null,
                NewAccountName(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<string>(),
                NewMoney(45645),
                NewPercentageRate(23)));
    }

    /// <summary>
    /// Tests that for Line of Credit Accounts, a new DEPOSIT transaction will decrease the Balance owed.
    /// </summary>
    [Fact]
    public void CanCall_ApplyTransactionAmount_DEPOSIT()
    {
        var value = NewMoney(1000);

        var previousValue = _testClass.Balance;

        _testClass.AddTransactionAmount(value, TransactionEnums.TransactionKeys.DEPOSIT);

        // Assert
        Assert.Equal(previousValue - value, _testClass.Balance);
    }

    /// <summary>
    /// Tests that for Line of Credit Accounts, a new PURCHASE transaction will increase the Balance.
    /// </summary>
    [Fact]
    public void CanCall_ApplyTransactionAmount_PURCHASE()
    {
        var value = NewMoney(1000);

        var previousValue = _testClass.Balance;

        _testClass.AddTransactionAmount(value, TransactionEnums.TransactionKeys.PURCHASE);

        // Assert
        Assert.Equal(previousValue + value, _testClass.Balance);
    }


    /// <summary>
    /// Tests that for Line of Credit Accounts, a previous DEPOSIT transaction will INCREASE the Balance.
    /// </summary>
    [Fact]
    public void CanCall_UndoTransactionAmount_DEPOSIT()
    {
        var value = NewMoney(1000);

        var previousValue = _testClass.Balance;

        _testClass.RemoveTransactionAmount(value, TransactionEnums.TransactionKeys.DEPOSIT);

        // Assert
        Assert.Equal(previousValue + value, _testClass.Balance);
    }

    /// <summary>
    /// Tests that for Line of Credit Accounts, a previous PURCHASE transaction will DECREASE the Balance.
    /// </summary>
    [Fact]
    public void CanCall_UndoTransactionAmount_PURCHASE()
    {
        var value = NewMoney(1000);

        var previousValue = _testClass.Balance;

        _testClass.RemoveTransactionAmount(value, TransactionEnums.TransactionKeys.PURCHASE);

        // Assert
        Assert.Equal(previousValue - value, _testClass.Balance);
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
            _testClass.RemoveTransactionAmount(new Money(1), TransactionEnums.TransactionKeys.DEPOSIT));
    }

    [Fact]
    public void CanCall_UpdateInterestRate()
    {
        var previousValue = _testClass.InterestRate;

        var newValue = new PercentageRate(10);

        Assert.Throws<NotImplementedException>(() => _testClass.UpdateInterestRate(newValue));

        return;

        _testClass.UpdateInterestRate(newValue);

        Assert.Equal(newValue, _testClass.InterestRate);
    }

    [Fact]
    public void CannotCall_UpdateInterestRate_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<NotImplementedException>(() => _testClass.UpdateInterestRate(new PercentageRate(1)));

        return;

        Assert.Throws<InvalidOperationException>(() => _testClass.UpdateInterestRate(new PercentageRate(1)));
    }



    [Fact]
    public void CanCall_UpdateCreditLimit()
    {
        var previousValue = _testClass.CreditLimit;

        var newValue = NewMoney(previousValue + 1000);

        Assert.Throws<NotImplementedException>(() => _testClass.UpdateCreditLimit(newValue));

        return;

        _testClass.UpdateCreditLimit(newValue);

        Assert.Equal(newValue, _testClass.CreditLimit);
    }

    [Fact]
    public void CannotCall_UpdateCreditLimit_When_IsDeleted()
    {
        _testClass.Delete();

        Assert.Throws<NotImplementedException>(() => _testClass.UpdateCreditLimit(NewMoney(1)));

        return;

        Assert.Throws<InvalidOperationException>(() => _testClass.UpdateCreditLimit(NewMoney(1)));
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