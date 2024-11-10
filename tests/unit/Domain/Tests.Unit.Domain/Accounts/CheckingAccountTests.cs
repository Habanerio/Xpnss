using System.Globalization;
using AutoFixture;
using Habanerio.Xpnss.Domain.Accounts;

namespace Habanerio.Xpnss.Tests.Unit.Domain.Accounts;

public class CheckingAccountTests : AccountTests
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
        Assert.Equal(overDraftAmount, result.OverDraftAmount);
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
        Assert.Equal(overDraftAmount, result.OverDraftAmount);
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

    [Fact]
    public void CanCall_AdjustBalance()
    {
        // Arrange
        var oldValue = _testClass.Balance;
        var newValue = NewMoney(4345634);
        var dateChanged = DateTime.Now.AddDays(-1);
        var reason = AutoFixture.Create<string>();

        // Act
        _testClass.AdjustBalance(newValue, dateChanged, reason);

        // Assert
        Assert.Equal(newValue, _testClass.Balance);
        Assert.NotEmpty(_testClass.ChangeHistory);

        var changeHistory = _testClass.ChangeHistory.Last();
        Assert.NotNull(changeHistory);
        Assert.Equal(_testClass.Id.Value, changeHistory.AccountId);
        Assert.Equal(_testClass.UserId.Value, changeHistory.UserId);
        Assert.Equal(nameof(_testClass.Balance), changeHistory.Property);
        Assert.Equal(oldValue.Value.ToString(CultureInfo.InvariantCulture), changeHistory.OldValue);
        Assert.Equal(newValue.Value.ToString(CultureInfo.InvariantCulture), changeHistory.NewValue);
        Assert.Equal(dateChanged.ToUniversalTime(), changeHistory.DateChanged);
        Assert.Equal(reason, changeHistory.Reason);
    }


    [Fact]
    public void CanCall_AdjustOverDraftAmount()
    {
        // Arrange
        var oldValue = _testClass.OverDraftAmount;
        var newValue = NewMoney(4345634);
        var dateChanged = DateTime.Now.AddDays(-1);
        var reason = AutoFixture.Create<string>();

        // Act
        _testClass.AdjustOverDraftAmount(newValue, dateChanged, reason);

        // Assert
        Assert.Equal(newValue, _testClass.OverDraftAmount);
        Assert.NotEmpty(_testClass.ChangeHistory);

        var changeHistory = _testClass.ChangeHistory.Last();
        Assert.NotNull(changeHistory);
        Assert.Equal(_testClass.Id.Value, changeHistory.AccountId);
        Assert.Equal(_testClass.UserId.Value, changeHistory.UserId);
        Assert.Equal(nameof(_testClass.OverDraftAmount), changeHistory.Property);
        Assert.Equal(oldValue.Value.ToString(CultureInfo.InvariantCulture), changeHistory.OldValue);
        Assert.Equal(newValue.Value.ToString(CultureInfo.InvariantCulture), changeHistory.NewValue);
        Assert.Equal(dateChanged.ToUniversalTime(), changeHistory.DateChanged);
        Assert.Equal(reason, changeHistory.Reason);
    }


    [Fact]
    public void CanGet_IsOverLimit()
    {
        // Arrange
        // TODO: Set up the test

        // Assert
        Assert.IsType<bool>(_testClass.IsOverLimit);

        //throw new NotImplementedException("Create or modify test");
    }
}