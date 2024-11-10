using System.Globalization;
using AutoFixture;
using Habanerio.Xpnss.Domain.Accounts;

namespace Habanerio.Xpnss.Tests.Unit.Domain.Accounts;

public class SavingsAccountTests : AccountTests
{
    private readonly SavingsAccount _testClass;

    public SavingsAccountTests()
    {
        var userId = NewUserId();
        var accountName = NewAccountName();
        var description = AutoFixture.Create<string>();
        var displayColor = "#C8C8C8";
        var interestRate = NewPercentageRate(23);

        // Act
        _testClass = SavingsAccount.New(userId, accountName, description, displayColor, interestRate);
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
        var dateCreated = AutoFixture.Create<DateTime>();
        var dateClosed = AutoFixture.Create<DateTime?>();
        var dateDeleted = AutoFixture.Create<DateTime?>();
        var dateUpdated = AutoFixture.Create<DateTime?>();

        var interestRate = NewPercentageRate(34);

        // Act
        var result = SavingsAccount.Load(id, userId, accountName, balance, description, displayColor, interestRate, dateCreated, dateClosed, dateDeleted, dateUpdated);

        // Assert
        Assert.Equal(id, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(accountName, result.Name);
        Assert.Equal(balance, result.Balance);
        Assert.Equal(description, result.Description);
        Assert.Equal(displayColor, result.DisplayColor);
        Assert.Equal(dateCreated, result.DateCreated);
        Assert.Equal(dateClosed, result.DateClosed);
        Assert.Equal(dateDeleted, result.DateDeleted);
        Assert.Equal(dateUpdated, result.DateUpdated);
        Assert.Equal(interestRate, result.InterestRate);
    }

    [Fact]
    public void CannotCall_Load_WithNull_Id()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SavingsAccount.Load(
                null,
                NewUserId(),
                NewAccountName(),
                NewMoney(4564),
                AutoFixture.Create<string>(),
                "#110022",
                NewPercentageRate(34),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));
    }

    [Fact]
    public void CannotCall_Load_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SavingsAccount.Load(
                NewAccountId(),
                null,
                NewAccountName(),
                NewMoney(4564),
                AutoFixture.Create<string>(),
                "#110022",
                NewPercentageRate(34),
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
        var interestRate = NewPercentageRate(23);

        // Act
        var result = SavingsAccount.New(userId, accountName, description, displayColor, interestRate);

        // Assert
        Assert.True(!string.IsNullOrWhiteSpace(result.Id.Value));
        Assert.Equal(userId, result.UserId);
        Assert.Equal(accountName, result.Name);
        Assert.Equal(0, result.Balance.Value);
        Assert.Equal(description, result.Description);
        Assert.Equal(displayColor, result.DisplayColor);
        Assert.Equal(interestRate, result.InterestRate);
    }

    [Fact]
    public void CannotCall_New_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SavingsAccount.New(
                null,
                NewAccountName(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<string>(),
                NewPercentageRate(23)));
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
    public void CanCall_AdjustInterestRate()
    {
        // Arrange
        var oldValue = _testClass.InterestRate;
        var newValue = NewPercentageRate(11);
        var dateChanged = DateTime.Now.AddDays(-1);
        var reason = AutoFixture.Create<string>();

        // Act
        _testClass.AdjustInterestRate(newValue, dateChanged, reason);

        // Assert
        Assert.Equal(newValue, _testClass.InterestRate);
        Assert.NotEmpty(_testClass.ChangeHistory);

        var changeHistory = _testClass.ChangeHistory.Last();
        Assert.NotNull(changeHistory);
        Assert.Equal(_testClass.Id.Value, changeHistory.AccountId);
        Assert.Equal(_testClass.UserId.Value, changeHistory.UserId);
        Assert.Equal(nameof(_testClass.InterestRate), changeHistory.Property);
        Assert.Equal(oldValue.Value.ToString(CultureInfo.InvariantCulture), changeHistory.OldValue);
        Assert.Equal(newValue.Value.ToString(CultureInfo.InvariantCulture), changeHistory.NewValue);
        Assert.Equal(dateChanged.ToUniversalTime(), changeHistory.DateChanged);
        Assert.Equal(reason, changeHistory.Reason);
    }
}