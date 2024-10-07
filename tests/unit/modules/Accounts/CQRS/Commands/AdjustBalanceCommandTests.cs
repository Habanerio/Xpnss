using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MongoDB.Bson;
using Moq;

namespace Habanerio.Xpnss.Tests.Unit.Modules.Accounts.CQRS.Commands;

public class AdjustBalanceCommandTests
{
    private readonly Mock<IAccountsRepository> _repository;

    private readonly AdjustBalance.Handler _handler;

    public AdjustBalanceCommandTests()
    {
        _repository = new Mock<IAccountsRepository>();

        _handler = new AdjustBalance.Handler(_repository.Object);
    }

    [Fact]
    public void Cannot_Instantiate_Handler_WithNull_Repository_ThrowsException()
    {
        IAccountsRepository repository = null;

        var error = Assert.Throws<ArgumentNullException>(() =>
            new AdjustBalance.Handler(repository));

        Assert.Equal("Value cannot be null. (Parameter 'repository')", error.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_AddAccount_WithInvalid_UserId_ReturnsFailed(string? value)
    {
        var command = new AdjustBalance.Command(
            value,
            ObjectId.GenerateNewId().ToString(),
            5000);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("'User Id' must not be empty.", result.Errors[0].Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_AddAccount_WithInvalid_AccountId_ReturnsFailed(string? value)
    {
        var command = new AdjustBalance.Command(
            "1",
            value,
            5000);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("'Account Id' must not be empty.", result.Errors[0].Message);
    }
}