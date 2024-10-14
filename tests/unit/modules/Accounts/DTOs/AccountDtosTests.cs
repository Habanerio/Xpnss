using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Xunit;

namespace Habanerio.Xpnss.Tests.Unit.Modules.Accounts.DTOs;

public class CashAccountDtoTests
{
    private readonly CashAccountDto _testClass;
    private IFixture _fixture;
    private string _id;
    private string _userId;
    private string _name;
    private decimal _balance;
    private string _description;
    private string _displayColor;
    private DateTimeOffset _dateCreated;
    private DateTimeOffset? _dateUpdated;
    private DateTimeOffset? _dateDeleted;

    public CashAccountDtoTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _id = _fixture.Create<string>();
        _userId = _fixture.Create<string>();
        _name = _fixture.Create<string>();
        _balance = _fixture.Create<decimal>();
        _description = _fixture.Create<string>();
        _displayColor = "#ff0000";
        _dateCreated = _fixture.Create<DateTimeOffset>();
        _dateUpdated = _fixture.Create<DateTimeOffset?>();
        _dateDeleted = _fixture.Create<DateTimeOffset?>();
    }

    [Fact]
    public void Can_Construct()
    {
        // Act
        var instance = new CashAccountDto(_id, _userId, _name, _description, _balance, _displayColor, _dateCreated, _dateUpdated, _dateDeleted);

        // Assert
        Assert.NotNull(instance);
    }

    [Fact]
    public void CanCall_New()
    {
        // Arrange
        var userId = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var description = _fixture.Create<string>();
        var balance = _fixture.Create<decimal>();
        var displayColor = "#ff0000";

        // Act
        var result = CashAccountDto.New(userId, name, description, balance, displayColor);

        // Assert
        Assert.NotNull(result);
        var account = Assert.IsType<CashAccountDto>(result);

        Assert.Equal(string.Empty, account.Id);
        Assert.Equal(userId, account.UserId);
        Assert.Equal(name, account.Name);
        Assert.Equal(nameof(AccountTypes.Cash), account.AccountType);
        Assert.Equal(description, account.Description);
        Assert.Equal(balance, account.Balance);
        Assert.Equal(displayColor, account.DisplayColor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_UserId(string value)
    {
        Assert.Throws<ArgumentException>(() =>
            CashAccountDto.New(value, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_Name(string value)
    {
        Assert.Throws<ArgumentException>(() =>
            CashAccountDto.New(_fixture.Create<string>(), value, _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData("ff0000")]
    [InlineData("#ff00ff00")]
    public void CannotCall_New_WithInvalid_DisplayColor(string value)
    {
        Assert.Throws<ArgumentException>(() =>
            CashAccountDto.New(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<decimal>(), value));
    }
}

public class CheckingAccountDtoTests
{
    private readonly CheckingAccountDto _testClass;
    private IFixture _fixture;
    private string _id;
    private string _userId;
    private string _name;
    private decimal _balance;
    private string _description;
    private string _displayColor;
    private decimal _overDraftAmount;
    private DateTimeOffset _dateCreated;
    private DateTimeOffset? _dateUpdated;
    private DateTimeOffset? _dateDeleted;

    public CheckingAccountDtoTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _id = _fixture.Create<string>();
        _userId = _fixture.Create<string>();
        _name = _fixture.Create<string>();
        _balance = _fixture.Create<decimal>();
        _description = _fixture.Create<string>();
        _displayColor = "#ff0000";
        _overDraftAmount = _fixture.Create<decimal>();
        _dateCreated = _fixture.Create<DateTimeOffset>();
        _dateUpdated = _fixture.Create<DateTimeOffset?>();
        _dateDeleted = _fixture.Create<DateTimeOffset?>();

        _testClass = new CheckingAccountDto(_id, _userId, _name, _description, _balance, _overDraftAmount, _displayColor, _dateCreated, _dateUpdated, _dateDeleted);
    }

    [Fact]
    public void Can_Construct()
    {
        // Act
        var instance = new CheckingAccountDto(_id, _userId, _name, _description, _balance, _overDraftAmount, _displayColor, _dateCreated, _dateUpdated, _dateDeleted);

        // Assert
        Assert.NotNull(instance);
        var account = Assert.IsType<CheckingAccountDto>(instance);

        Assert.Equal(_id, account.Id);
        Assert.Equal(_userId, account.UserId);
        Assert.Equal(_name, account.Name);
        Assert.Equal(nameof(AccountTypes.Checking), account.AccountType);
        Assert.Equal(_balance, account.Balance);
        Assert.Equal(_description, account.Description);
        Assert.Equal(_displayColor, account.DisplayColor);
        Assert.Equal(_overDraftAmount, account.OverDraftAmount);
        Assert.Equal(_dateCreated, account.DateCreated);
        Assert.Equal(_dateUpdated, account.DateUpdated);
        Assert.Equal(_dateDeleted, account.DateDeleted);
    }

    [Fact]
    public void CanCall_New()
    {
        // Act
        var result = CheckingAccountDto.New(_userId, _name, _description, _balance, _overDraftAmount, _displayColor);

        // Assert
        Assert.NotNull(result);
        var account = Assert.IsType<CheckingAccountDto>(result);

        Assert.Equal(string.Empty, account.Id);
        Assert.Equal(_userId, account.UserId);
        Assert.Equal(_name, account.Name);
        Assert.Equal(nameof(AccountTypes.Checking), account.AccountType);
        Assert.Equal(_balance, account.Balance);
        Assert.Equal(_description, account.Description);
        Assert.Equal(_displayColor, account.DisplayColor);
        Assert.Equal(_overDraftAmount, account.OverDraftAmount);

        //Assert.Equal(_dateCreated, account.DateCreated, );
        Assert.Null(account.DateUpdated);
        Assert.Null(account.DateDeleted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_UserId(string value)
    {
        Assert.Throws<ArgumentException>(() => CheckingAccountDto.New(value, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_Name(string value)
    {
        Assert.Throws<ArgumentException>(() => CheckingAccountDto.New(_fixture.Create<string>(), value, _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData("ff0000")]
    [InlineData("#ff00ff00")]
    public void CannotCall_New_WithInvalid_DisplayColor(string value)
    {
        Assert.Throws<ArgumentException>(() => CheckingAccountDto.New(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), value));
    }

    [Fact]
    public void CanSet_And_Get_OverDraftAmount()
    {
        // Arrange
        var testValue = _fixture.Create<decimal>();

        // Act
        _testClass.OverDraftAmount = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.OverDraftAmount);
    }
}

public class SavingsAccountDtoTests
{
    private readonly SavingsAccountDto _testClass;
    private IFixture _fixture;
    private string _id;
    private string _userId;
    private string _name;
    private decimal _balance;
    private string _description;
    private string _displayColor;
    private decimal _interestRate;
    private DateTimeOffset _dateCreated;
    private DateTimeOffset? _dateUpdated;
    private DateTimeOffset? _dateDeleted;

    public SavingsAccountDtoTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _id = _fixture.Create<string>();
        _userId = _fixture.Create<string>();
        _name = _fixture.Create<string>();
        _balance = _fixture.Create<decimal>();
        _description = _fixture.Create<string>();
        _displayColor = "#ff0000";
        _interestRate = _fixture.Create<decimal>();
        _dateCreated = _fixture.Create<DateTimeOffset>();
        _dateUpdated = _fixture.Create<DateTimeOffset?>();
        _dateDeleted = _fixture.Create<DateTimeOffset?>();

        _testClass = new SavingsAccountDto(_id, _userId, _name, _description, _balance, _interestRate,
            _displayColor, _dateCreated, _dateUpdated, _dateDeleted);
    }

    [Fact]
    public void Can_Construct()
    {
        // Act
        var instance = new SavingsAccountDto(_id, _userId, _name, _description, _balance, _interestRate, _displayColor, _dateCreated, _dateUpdated, _dateDeleted);

        // Assert
        Assert.NotNull(instance);

        var account = Assert.IsType<SavingsAccountDto>(instance);
        Assert.Equal(_id, account.Id);
        Assert.Equal(_userId, account.UserId);
        Assert.Equal(_name, account.Name);
        Assert.Equal(nameof(AccountTypes.Savings), account.AccountType);
        Assert.Equal(_balance, account.Balance);
        Assert.Equal(_description, account.Description);
        Assert.Equal(_displayColor, account.DisplayColor);
        Assert.Equal(_interestRate, account.InterestRate);
        Assert.Equal(_dateCreated, account.DateCreated);
        Assert.Equal(_dateUpdated, account.DateUpdated);
        Assert.Equal(_dateDeleted, account.DateDeleted);
    }


    [Fact]
    public void CanCall_New()
    {
        // Act
        var result = SavingsAccountDto.New(_userId, _name, _description, _balance, _interestRate, _displayColor);

        // Assert
        Assert.NotNull(result);

        var account = Assert.IsType<SavingsAccountDto>(result);
        Assert.Equal(string.Empty, account.Id);
        Assert.Equal(_userId, account.UserId);
        Assert.Equal(_name, account.Name);
        Assert.Equal(nameof(AccountTypes.Savings), account.AccountType);
        Assert.Equal(_balance, account.Balance);
        Assert.Equal(_description, account.Description);
        Assert.Equal(_displayColor, account.DisplayColor);
        Assert.Equal(_interestRate, account.InterestRate);
        //Assert.Equal(_dateCreated, account.DateCreated);
        Assert.Null(account.DateUpdated);
        Assert.Null(account.DateDeleted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_UserId(string value)
    {
        Assert.Throws<ArgumentException>(() => SavingsAccountDto.New(value, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_Name(string value)
    {
        Assert.Throws<ArgumentException>(() => SavingsAccountDto.New(_fixture.Create<string>(), value, _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData("ff0000")]
    [InlineData("#ff00ff00")]
    public void CannotCall_New_WithInvalid_DisplayColor(string value)
    {
        Assert.Throws<ArgumentException>(() => SavingsAccountDto.New(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), value));
    }

    [Fact]
    public void CanSet_And_Get_InterestRate()
    {
        // Arrange
        var testValue = _fixture.Create<decimal>();

        // Act
        _testClass.InterestRate = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.InterestRate);
    }
}

public class CreditCardAccountDtoTests
{
    private readonly CreditCardAccountDto _testClass;
    private IFixture _fixture;
    private readonly string _id;
    private readonly string _userId;
    private readonly string _name;
    private readonly decimal _balance;
    private readonly string _description;
    private readonly string _displayColor;
    private readonly decimal _creditLimit;
    private readonly decimal _interestRate;
    private readonly DateTimeOffset _dateCreated;
    private readonly DateTimeOffset? _dateUpdated;
    private readonly DateTimeOffset? _dateDeleted;

    public CreditCardAccountDtoTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _id = _fixture.Create<string>();
        _userId = _fixture.Create<string>();
        _name = _fixture.Create<string>();
        _balance = _fixture.Create<decimal>();
        _description = _fixture.Create<string>();
        _displayColor = "#ff0000";
        _creditLimit = _fixture.Create<decimal>();
        _interestRate = _fixture.Create<decimal>();
        _dateCreated = _fixture.Create<DateTimeOffset>();
        _dateUpdated = _fixture.Create<DateTimeOffset?>();
        _dateDeleted = _fixture.Create<DateTimeOffset?>();

        _testClass = new CreditCardAccountDto(_id, _userId, _name, _description, _balance, _creditLimit, _interestRate, _displayColor, _dateCreated, _dateUpdated, _dateDeleted);
    }

    [Fact]
    public void Can_Construct()
    {
        // Act
        var instance = new CreditCardAccountDto(_id, _userId, _name, _description, _balance, _creditLimit, _interestRate, _displayColor, _dateCreated, _dateUpdated, _dateDeleted);

        // Assert
        Assert.NotNull(instance);

        var account = Assert.IsType<CreditCardAccountDto>(instance);
        Assert.Equal(_id, account.Id);
        Assert.Equal(_userId, account.UserId);
        Assert.Equal(_name, account.Name);
        Assert.Equal(nameof(AccountTypes.CreditCard), account.AccountType);
        Assert.Equal(_balance, account.Balance);
        Assert.Equal(_description, account.Description);
        Assert.Equal(_displayColor, account.DisplayColor);
        Assert.Equal(_creditLimit, account.CreditLimit);
        Assert.Equal(_interestRate, account.InterestRate);
        Assert.Equal(_dateCreated, account.DateCreated);
        Assert.Equal(_dateUpdated, account.DateUpdated);
        Assert.Equal(_dateDeleted, account.DateDeleted);
    }

    [Fact]
    public void CanCall_New()
    {
        // Act
        var result = CreditCardAccountDto.New(_userId, _name, _description, _balance, _creditLimit, _interestRate, _displayColor);

        // Assert
        Assert.NotNull(result);

        var account = Assert.IsType<CreditCardAccountDto>(result);

        Assert.Equal(string.Empty, account.Id);
        Assert.Equal(_userId, account.UserId);
        Assert.Equal(_name, account.Name);
        Assert.Equal(nameof(AccountTypes.CreditCard), account.AccountType);
        Assert.Equal(_balance, account.Balance);
        Assert.Equal(_description, account.Description);
        Assert.Equal(_displayColor, account.DisplayColor);
        Assert.Equal(_creditLimit, account.CreditLimit);
        Assert.Equal(_interestRate, account.InterestRate);
        ////Assert.Equal(_dateCreated, account.DateCreated);
        Assert.Null(account.DateUpdated);
        Assert.Null(account.DateDeleted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_UserId(string value)
    {
        Assert.Throws<ArgumentException>(() => CreditCardAccountDto.New(value, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_Name(string value)
    {
        Assert.Throws<ArgumentException>(() => CreditCardAccountDto.New(_fixture.Create<string>(), value, _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData("ff0000")]
    [InlineData("#ff00ff00")]
    public void CannotCall_New_WithInvalid_DisplayColor(string value)
    {
        Assert.Throws<ArgumentException>(() => CreditCardAccountDto.New(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), value));
    }
}

public class LineOfCreditAccountDtoTests
{
    private readonly LineOfCreditAccountDto _testClass;
    private readonly IFixture _fixture;
    private readonly string _id;
    private readonly string _userId;
    private readonly string _name;
    private readonly decimal _balance;
    private readonly string _description;
    private readonly string _displayColor;
    private readonly decimal _creditLimit;
    private readonly decimal _interestRate;
    private readonly DateTimeOffset _dateCreated;
    private readonly DateTimeOffset? _dateUpdated;
    private readonly DateTimeOffset? _dateDeleted;

    public LineOfCreditAccountDtoTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _id = _fixture.Create<string>();
        _userId = _fixture.Create<string>();
        _name = _fixture.Create<string>();
        _balance = _fixture.Create<decimal>();
        _description = _fixture.Create<string>();
        _displayColor = "#ff0000";
        _creditLimit = _fixture.Create<decimal>();
        _interestRate = _fixture.Create<decimal>();
        _dateCreated = _fixture.Create<DateTimeOffset>();
        _dateUpdated = _fixture.Create<DateTimeOffset?>();
        _dateDeleted = _fixture.Create<DateTimeOffset?>();

        _testClass = new LineOfCreditAccountDto(_id, _userId, _name, _description, _balance, _creditLimit, _interestRate, _displayColor, _dateCreated, _dateUpdated, _dateDeleted);
    }

    [Fact]
    public void Can_Construct()
    {
        // Act
        var instance = new LineOfCreditAccountDto(_id, _userId, _name, _description, _balance, _creditLimit, _interestRate, _displayColor, _dateCreated, _dateUpdated, _dateDeleted);

        // Assert
        Assert.NotNull(instance);

        var account = Assert.IsType<LineOfCreditAccountDto>(instance);
        Assert.Equal(_id, account.Id);
        Assert.Equal(_userId, account.UserId);
        Assert.Equal(_name, account.Name);
        Assert.Equal(nameof(AccountTypes.LineOfCredit), account.AccountType);
        Assert.Equal(_balance, account.Balance);
        Assert.Equal(_description, account.Description);
        Assert.Equal(_displayColor, account.DisplayColor);
        Assert.Equal(_creditLimit, account.CreditLimit);
        Assert.Equal(_interestRate, account.InterestRate);
        Assert.Equal(_dateCreated, account.DateCreated);
        Assert.Equal(_dateUpdated, account.DateUpdated);
        Assert.Equal(_dateDeleted, account.DateDeleted);
    }

    [Fact]
    public void CanCall_New()
    {
        // Arrange
        var userId = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var description = _fixture.Create<string>();
        var balance = _fixture.Create<decimal>();
        var creditLimit = _fixture.Create<decimal>();
        var interestRate = _fixture.Create<decimal>();
        var displayColor = "#ff0000";

        // Act
        var result = LineOfCreditAccountDto.New(_userId, _name, _description, _balance, _creditLimit, _interestRate, _displayColor);

        // Assert
        Assert.NotNull(result);

        var account = Assert.IsType<LineOfCreditAccountDto>(result);

        Assert.Equal(string.Empty, account.Id);
        Assert.Equal(_userId, account.UserId);
        Assert.Equal(_name, account.Name);
        Assert.Equal(nameof(AccountTypes.LineOfCredit), account.AccountType);
        Assert.Equal(_balance, account.Balance);
        Assert.Equal(_description, account.Description);
        Assert.Equal(_displayColor, account.DisplayColor);
        Assert.Equal(_creditLimit, account.CreditLimit);
        Assert.Equal(_interestRate, account.InterestRate);
        //Assert.Equal(_dateCreated, account.DateCreated);
        Assert.Null(account.DateUpdated);
        Assert.Null(account.DateDeleted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_UserId(string value)
    {
        Assert.Throws<ArgumentException>(() => LineOfCreditAccountDto.New(value, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_Name(string value)
    {
        Assert.Throws<ArgumentException>(() => LineOfCreditAccountDto.New(_fixture.Create<string>(), value, _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<string>()));
    }

    [Theory]
    [InlineData("ff0000")]
    [InlineData("#ff00ff00")]
    public void CannotCall_New_WithInvalid_DisplayColor(string value)
    {
        Assert.Throws<ArgumentException>(() => LineOfCreditAccountDto.New(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), _fixture.Create<decimal>(), value));
    }
}

/*
public class AccountDtoTests
{
    private readonly AccountDto _testClass;
    private IFixture _fixture;
    private AccountTypes _accountType;
    private bool _isCredit;
    private string _id;
    private string _userId;
    private string _name;
    private decimal _balance;
    private string _description;
    private string _displayColor;
    private DateTimeOffset _dateCreated;
    private DateTimeOffset? _dateUpdated;
    private DateTimeOffset? _dateDeleted;

    public AccountDtoTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _accountType = _fixture.Create<AccountTypes>();
        _isCredit = _fixture.Create<bool>();
        _id = _fixture.Create<string>();
        _userId = _fixture.Create<string>();
        _name = _fixture.Create<string>();
        _balance = _fixture.Create<decimal>();
        _description = _fixture.Create<string>();
        _displayColor = "#ff0000";
        _dateCreated = _fixture.Create<DateTimeOffset>();
        _dateUpdated = _fixture.Create<DateTimeOffset?>();
        _dateDeleted = _fixture.Create<DateTimeOffset?>();
        _testClass = _fixture.Create<AccountDto>();
    }

    [Fact]
    public void Can_Construct()
    {
        // Act
        var instance = new AccountDto(_id, _userId, _name, _balance, _description, _displayColor, _isCredit, _dateCreated, _dateUpdated, _dateDeleted);

        // Assert
        Assert.NotNull(instance);
    }

    [Fact]
    public void Implements_IEquatable__AccountDto()
    {
        // Arrange
        var same = new AccountDto(_id, _userId, _name, _balance, _description, _displayColor, _isCredit, _dateCreated, _dateUpdated, _dateDeleted);
        var different = _fixture.Create<AccountDto>();

        // Assert
        Assert.False(_testClass.Equals(default(object)));
        Assert.False(_testClass.Equals(new object()));
        Assert.True(_testClass.Equals((object)same));
        Assert.False(_testClass.Equals((object)different));
        Assert.True(_testClass.Equals(same));
        Assert.False(_testClass.Equals(different));
        Assert.Equal(same.GetHashCode(), _testClass.GetHashCode());
        Assert.NotEqual(different.GetHashCode(), _testClass.GetHashCode());
        Assert.True(_testClass == same);
        Assert.False(_testClass == different);
        Assert.False(_testClass != same);
        Assert.True(_testClass != different);
    }

    [Fact]
    public void CanSet_And_Get_UserId()
    {
        // Arrange
        var testValue = _fixture.Create<string>();

        // Act
        _testClass.UserId = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.UserId);
    }

    [Fact]
    public void CanSet_And_Get_AccountType()
    {
        // Arrange
        var testValue = _fixture.Create<AccountTypes>();

        // Act
        _testClass.AccountTypes = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.AccountTypes);
    }

    [Fact]
    public void CanSet_And_Get_Name()
    {
        // Arrange
        var testValue = _fixture.Create<string>();

        // Act
        _testClass.Name = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.Name);
    }

    [Fact]
    public void CanSet_And_Get_Balance()
    {
        // Arrange
        var testValue = _fixture.Create<decimal>();

        // Act
        _testClass.Balance = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.Balance);
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
    public void CanSet_And_Get_IsCredit()
    {
        // Arrange
        var testValue = _fixture.Create<bool>();

        // Act
        _testClass.IsCredit = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.IsCredit);
    }

    [Fact]
    public void CanGet_IsDeleted()
    {
        // Assert
        Assert.IsType<bool>(_testClass.IsDeleted);

        throw new NotImplementedException("Create or modify test");
    }

    [Fact]
    public void CanSet_And_Get_DateCreated()
    {
        // Arrange
        var testValue = _fixture.Create<DateTimeOffset>();

        // Act
        _testClass.DateCreated = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.DateCreated);
    }

    [Fact]
    public void CanSet_And_Get_DateUpdated()
    {
        // Arrange
        var testValue = _fixture.Create<DateTimeOffset?>();

        // Act
        _testClass.DateUpdated = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.DateUpdated);
    }

    [Fact]
    public void CanSet_And_Get_DateDeleted()
    {
        // Arrange
        var testValue = _fixture.Create<DateTimeOffset?>();

        // Act
        _testClass.DateDeleted = testValue;

        // Assert
        Assert.Equal(testValue, _testClass.DateDeleted);
    }
}
*/