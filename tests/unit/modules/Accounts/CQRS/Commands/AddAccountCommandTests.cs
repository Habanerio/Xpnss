//using Habanerio.Xpnss.Modules.Accounts.Common;
//using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
//using Habanerio.Xpnss.Modules.Accounts.Interfaces;
//using Moq;

//namespace Habanerio.Xpnss.Tests.Unit.Modules.Accounts.CQRS.Commands;

//public class AddAccountCommandTests
//{
//    private readonly Mock<IAccountsRepository> _repository;

//    private readonly AddAccount.Handler _handler;

//    public AddAccountCommandTests()
//    {
//        _repository = new Mock<IAccountsRepository>();

//        _handler = new AddAccount.Handler(_repository.Object);
//    }

//    [Fact]
//    public void Cannot_Instantiate_Handler_WithNull_Repository_ThrowsException()
//    {
//        IAccountsRepository repository = null;

//        var error = Assert.Throws<ArgumentNullException>(() =>
//            new AddAccount.Handler(repository));

//        Assert.Equal("Value cannot be null. (Parameter 'repository')", error.Message);
//    }

//    [Fact]
//    public async Task CannotCall_AddAccount_WithNull_Request_ThrowsException()
//    {
//        AddAccount.Command? command = null;

//        var error = await Assert.ThrowsAsync<ArgumentNullException>(() =>
//            _handler.Handle(command, CancellationToken.None));

//        Assert.Equal("Cannot pass null model to Validate. (Parameter 'instanceToValidate')", error.Message);
//    }

//    [Theory]
//    [InlineData("")]
//    [InlineData(" ")]
//    [InlineData(null)]
//    public async Task CannotCall_AddAccount_WithInvalid_UserId_ReturnsFailed(string? value)
//    {
//        var command = new AddAccount.Command(
//            value,
//            AccountType.Cash,
//            "Cash Account",
//            "Cash Account Description",
//            100m,
//            DisplayColor: "#ff00ff"
//        );

//        var result = await _handler.Handle(command, CancellationToken.None);

//        Assert.True(result.IsFailed);
//        Assert.Equal("'User Id' must not be empty.", result.Errors[0].Message);
//    }

//    [Fact]
//    public async Task CannotCall_AddAccount_WithInvalid_AccountType_ReturnsFailed()
//    {
//        var command = new AddAccount.Command(
//            "1",
//            (AccountType)10000,
//            "Cash Account",
//            "Cash Account Description",
//            100m,
//            DisplayColor: "#ff00ff"
//        );

//        var result = await _handler.Handle(command, CancellationToken.None);

//        Assert.Equal("'Account Type' has a range of values which does not include '10000'.", result.Errors[0].Message);
//    }

//    [Theory]
//    [InlineData("")]
//    [InlineData(" ")]
//    [InlineData(null)]
//    public async Task CannotCall_AddAccount_WithInvalid_Name_ReturnsFailed(string? value)
//    {
//        var command = new AddAccount.Command(
//            "1",
//            AccountType.Cash,
//            value,
//            "Cash Account Description",
//            100m,
//            DisplayColor: "#ff00ff"
//        );

//        var result = await _handler.Handle(command, CancellationToken.None);

//        Assert.True(result.IsFailed);
//        Assert.Equal("'Name' must not be empty.", result.Errors[0].Message);
//    }
//}