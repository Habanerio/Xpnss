using AutoFixture;
using AutoFixture.AutoMoq;
using Habanerio.Xpnss.Domain.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Unit.Domain.Accounts;


public class BaseAccountTests
{
    protected IFixture AutoFixture;

    public BaseAccountTests()
    {
        AutoFixture = new Fixture().Customize(new AutoMoqCustomization());
        AutoFixture.Register(ObjectId.GenerateNewId);
    }

    public static AccountId NewAccountId()
    {
        return AccountId.New;
    }

    public static UserId NewUserId()
    {
        return new UserId(Guid.NewGuid().ToString());
    }

    public static AccountName NewAccountName()
    {
        return new AccountName(Guid.NewGuid().ToString());
    }

    public static Money NewMoney(decimal value)
    {
        return new Money(value);
    }

    public static PercentageRate NewPercentageRate(decimal value)
    {
        return new PercentageRate(value);
    }
}

/* Commenting out for now as they are abstract classes. Tests for actual Account classes should cover these.
{
    private class TestAccount : BaseAccount
    {
        public TestAccount(
        AccountId id,
        UserId userId,
        AccountTypes.Keys accountType,
        AccountName accountName,
        Money balance,
        string description,
        string displayColor,
        DateTime dateCreated,
        DateTime? dateClosed,
        DateTime? dateDeleted,
        DateTime? dateUpdated) : base(id, userId, accountType, accountName, balance, description, displayColor, dateCreated, dateClosed, dateDeleted, dateUpdated)
        {
        }
    }

    private readonly TestAccount _testClass;
    private IFixture _fixture;
    private AccountId _id;
    private UserId _userId;
    private AccountTypes.Keys _accountType;
    private AccountName _accountName;
    private Money _balance;
    private string _description;
    private string _displayColor;
    private DateTime _dateCreated;
    private DateTime? _dateClosed;
    private DateTime? _dateDeleted;
    private DateTime? _dateUpdated;

    public BaseAccountTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _id = _fixture.Create<AccountId>();
        _userId = _fixture.Create<UserId>();
        _accountType = _fixture.Create<AccountTypes.Keys>();
        _accountName = _fixture.Create<AccountName>();
        _balance = _fixture.Create<Money>();
        _description = _fixture.Create<string>();
        _displayColor = _fixture.Create<string>();
        _dateCreated = _fixture.Create<DateTime>();
        _dateClosed = _fixture.Create<DateTime?>();
        _dateDeleted = _fixture.Create<DateTime?>();
        _dateUpdated = _fixture.Create<DateTime?>();
        _testClass = _fixture.Create<TestAccount>();
    }

    [Fact]
    public void Cannot_Construct_WithNull_Id()
    {
        Assert.Throws<ArgumentNullException>(() => new TestAccount(default(AccountId), _userId, _accountType, _accountName, _balance, _description, _displayColor, _dateCreated, _dateClosed, _dateDeleted, _dateUpdated));
    }

    [Fact]
    public void Cannot_Construct_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() => new TestAccount(_id, default(UserId), _accountType, _accountName, _balance, _description, _displayColor, _dateCreated, _dateClosed, _dateDeleted, _dateUpdated));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Cannot_Construct_WithInvalid_Description(string value)
    {
        Assert.Throws<ArgumentNullException>(() => new TestAccount(_id, _userId, _accountType, _accountName, _balance, value, _displayColor, _dateCreated, _dateClosed, _dateDeleted, _dateUpdated));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Cannot_Construct_WithInvalid_DisplayColor(string value)
    {
        Assert.Throws<ArgumentNullException>(() => new TestAccount(_id, _userId, _accountType, _accountName, _balance, _description, value, _dateCreated, _dateClosed, _dateDeleted, _dateUpdated));
    }

    [Fact]
    public void CanCall_Deposit()
    {
        // Arrange
        var dateOfDeposit = _fixture.Create<DateTime>();
        var creditLimit = _fixture.Create<Money>();

        // Act
        _testClass.AddDeposit(dateOfDeposit, creditLimit);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCall_Withdraw()
    {
        // Arrange
        var dateOfWithdraw = _fixture.Create<DateTime>();
        var creditLimit = _fixture.Create<Money>();

        // Act
        _testClass.AddWithdrawal(dateOfWithdraw, creditLimit);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCall_AdjustBalance()
    {
        // Arrange
        var newValue = _fixture.Create<Money>();
        var dateChanged = _fixture.Create<DateTime>();
        var reason = _fixture.Create<string>();

        // Act
        _testClass.AddBalanceAdjustment(newValue, dateChanged, reason);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_AdjustBalance_WithInvalid_Reason(string value)
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.AddBalanceAdjustment(_fixture.Create<Money>(), _fixture.Create<DateTime>(), value));
    }

    [Fact]
    public void CanCall_UpdateDetails()
    {
        // Arrange
        var accountName = _fixture.Create<string>();
        var description = _fixture.Create<string>();
        var displayColour = _fixture.Create<string>();

        // Act
        _testClass.UpdateDetails(accountName, description, displayColour);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_UpdateDetails_WithInvalid_AccountName(string value)
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.UpdateDetails(value, _fixture.Create<string>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_UpdateDetails_WithInvalid_Description(string value)
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.UpdateDetails(_fixture.Create<string>(), value, _fixture.Create<string>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_UpdateDetails_WithInvalid_DisplayColour(string value)
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.UpdateDetails(_fixture.Create<string>(), _fixture.Create<string>(), value));
    }

    [Fact]
    public void CanCall_LoadChangeHistories()
    {
        // Arrange
        var changeHistories = _fixture.Create<List<AdjustmentHistories>>();

        // Act
        _testClass.LoadChangeHistories(changeHistories);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCall_LoadChangeHistories_WithNull_ChangeHistories()
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.LoadChangeHistories(default(List<AdjustmentHistories>)));
    }

    [Fact]
    public void CanCall_LoadMonthlyTotals()
    {
        // Arrange
        var monthlyTotals = _fixture.Create<List<MonthlyAccountTotal>>();

        // Act
        _testClass.LoadMonthlyTotals(monthlyTotals);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CannotCall_LoadMonthlyTotals_WithNull_MonthlyTotals()
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.LoadMonthlyTotals(default(List<MonthlyAccountTotal>)));
    }

    [Fact]
    public void CanGet_Name()
    {
        // Assert
        Assert.IsType<AccountName>(_testClass.Name);

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanSet_And_Get_Balance()
    {
        // Arrange
        var testValue = _fixture.Create<Money>();

        // Act
        _testClass.Balance = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.Balance);
    }

    [Fact]
    public void CanGet_ChangeHistory()
    {
        // Assert
        Assert.IsType<IReadOnlyCollection<AdjustmentHistories>>(_testClass.AdjustmentHistories);

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanSet_And_Get_Description()
    {
        // Arrange
        var testValue = _fixture.Create<string>();

        // Act
        _testClass.Description = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.Description);
    }

    [Fact]
    public void CanSet_And_Get_DisplayColor()
    {
        // Arrange
        var testValue = _fixture.Create<string>();

        // Act
        _testClass.DisplayColor = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.DisplayColor);
    }

    [Fact]
    public void CanGet_IsClosed()
    {
        // Assert
        Assert.IsType<bool>(_testClass.IsClosed);

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanSet_And_Get_IsCredit()
    {
        // Arrange
        var testValue = _fixture.Create<bool>();

        // Act
        _testClass.DoesBalanceIncrease = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.DoesBalanceIncrease);
    }

    [Fact]
    public void CanGet_MonthlyTotals()
    {
        // Assert
        Assert.IsType<IReadOnlyCollection<MonthlyAccountTotal>>(_testClass.BaseMonthlyTotalDocument);

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanSet_And_Get_DateUpdated()
    {
        // Arrange
        var testValue = _fixture.Create<DateTime?>();

        // Act
        _testClass.DateUpdated = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.DateUpdated);
    }
}

public class CreditAccountTests
{
    private class TestCreditAccount : BaseCreditAccount
    {
        public TestCreditAccount(AccountId id,
                UserId userId,
                AccountTypes.Keys accountType,
                AccountName accountName,
                Money balance,
                string description,
                string displayColor,
                Money creditLimit,
                PercentageRate interestRate,
                DateTime dateCreated,
                DateTime? dateClosed,
                DateTime? dateDeleted,
                DateTime? dateUpdated) : base(id, userId, accountType, accountName, balance, description, displayColor, creditLimit, interestRate, dateCreated, dateClosed, dateDeleted, dateUpdated)
        {
        }
    }

    private readonly TestCreditAccount _testClass;
    private IFixture _fixture;
    private AccountId _id;
    private UserId _userId;
    private AccountTypes.Keys _accountType;
    private AccountName _accountName;
    private Money _balance;
    private string _description;
    private string _displayColor;
    private Money _creditLimit;
    private PercentageRate _interestRate;
    private DateTime _dateCreated;
    private DateTime? _dateClosed;
    private DateTime? _dateDeleted;
    private DateTime? _dateUpdated;

    public CreditAccountTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _id = _fixture.Create<AccountId>();
        _userId = _fixture.Create<UserId>();
        _accountType = _fixture.Create<AccountTypes.Keys>();
        _accountName = _fixture.Create<AccountName>();
        _balance = _fixture.Create<Money>();
        _description = _fixture.Create<string>();
        _displayColor = _fixture.Create<string>();
        _creditLimit = _fixture.Create<Money>();
        _interestRate = _fixture.Create<PercentageRate>();
        _dateCreated = _fixture.Create<DateTime>();
        _dateClosed = _fixture.Create<DateTime?>();
        _dateDeleted = _fixture.Create<DateTime?>();
        _dateUpdated = _fixture.Create<DateTime?>();
        _testClass = _fixture.Create<TestCreditAccount>();
    }

    [Fact]
    public void Cannot_Construct_WithNull_Id()
    {
        Assert.Throws<ArgumentNullException>(() => new TestCreditAccount(default(AccountId), _userId, _accountType, _accountName, _balance, _description, _displayColor, _creditLimit, _interestRate, _dateCreated, _dateClosed, _dateDeleted, _dateUpdated));
    }

    [Fact]
    public void Cannot_Construct_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() => new TestCreditAccount(_id, default(UserId), _accountType, _accountName, _balance, _description, _displayColor, _creditLimit, _interestRate, _dateCreated, _dateClosed, _dateDeleted, _dateUpdated));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Cannot_Construct_WithInvalid_Description(string value)
    {
        Assert.Throws<ArgumentNullException>(() => new TestCreditAccount(_id, _userId, _accountType, _accountName, _balance, value, _displayColor, _creditLimit, _interestRate, _dateCreated, _dateClosed, _dateDeleted, _dateUpdated));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Cannot_Construct_WithInvalid_DisplayColor(string value)
    {
        Assert.Throws<ArgumentNullException>(() => new TestCreditAccount(_id, _userId, _accountType, _accountName, _balance, _description, value, _creditLimit, _interestRate, _dateCreated, _dateClosed, _dateDeleted, _dateUpdated));
    }

    [Fact]
    public void CanCall_Deposit()
    {
        // Arrange
        var dateOfDeposit = _fixture.Create<DateTime>();
        var creditLimit = _fixture.Create<Money>();

        // Act
        _testClass.AddDeposit(dateOfDeposit, creditLimit);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCall_Withdraw()
    {
        // Arrange
        var dateOfWithdraw = _fixture.Create<DateTime>();
        var creditLimit = _fixture.Create<Money>();

        // Act
        _testClass.AddWithdrawal(dateOfWithdraw, creditLimit);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanCall_AdjustCreditLimit()
    {
        // Arrange
        var newValue = _fixture.Create<Money>();
        var dateChanged = _fixture.Create<DateTime>();
        var reason = _fixture.Create<string>();

        // Act
        _testClass.AddCreditLimitAdjustment(newValue, dateChanged, reason);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_AdjustCreditLimit_WithInvalid_Reason(string value)
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.AddCreditLimitAdjustment(_fixture.Create<Money>(), _fixture.Create<DateTime>(), value));
    }

    [Fact]
    public void CanCall_AdjustInterestRate()
    {
        // Arrange
        var newValue = _fixture.Create<PercentageRate>();
        var dateChanged = _fixture.Create<DateTime>();
        var reason = _fixture.Create<string>();

        // Act
        _testClass.AddInterestRateAdjustment(newValue, dateChanged, reason);

        // Assert
        throw new NotImplementedException("Create or modify test");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_AdjustInterestRate_WithInvalid_Reason(string value)
    {
        Assert.Throws<ArgumentNullException>(() => _testClass.AddInterestRateAdjustment(_fixture.Create<PercentageRate>(), _fixture.Create<DateTime>(), value));
    }

    [Fact]
    public void CanSet_And_Get_CreditLimit()
    {
        // Arrange
        var testValue = _fixture.Create<Money>();

        // Act
        _testClass.CreditLimit = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.CreditLimit);
    }

    [Fact]
    public void CanSet_And_Get_InterestRate()
    {
        // Arrange
        var testValue = _fixture.Create<PercentageRate>();

        // Act
        _testClass.InterestRate = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.InterestRate);
    }

    [Fact]
    public void CanGet_IsOverLimit()
    {
        // Assert
        Assert.IsType<bool>(_testClass.IsOverLimit);

        throw new NotImplementedException("Create or modify test");
    }
}
*/