using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MongoDB.Bson;
using Moq;

namespace Habanerio.Xpnss.Tests.Unit.Modules.Accounts.CQRS.Commands;

public class AdjustInterestRateCommandTests
{
    private readonly Mock<IAccountsRepository> _repository;

    private readonly AdjustInterestRate.Handler _handler;

    public AdjustInterestRateCommandTests()
    {
        _repository = new Mock<IAccountsRepository>();

        _handler = new AdjustInterestRate.Handler(_repository.Object);
    }

    [Fact]
    public void Cannot_Instantiate_Handler_WithNull_Repository_ThrowsException()
    {
        IAccountsRepository repository = null;

        var error = Assert.Throws<ArgumentNullException>(() =>
            new AdjustCreditLimit.Handler(repository));

        Assert.Equal("Value cannot be null. (Parameter 'repository')", error.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_AddAccount_WithInvalid_UserId_ReturnsFailed(string? value)
    {
        var command = new AdjustInterestRate.Command(
            value,
            ObjectId.GenerateNewId().ToString(),
            12.99m);

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
        var command = new AdjustInterestRate.Command(
            "1",
            value,
            12.99m);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("'Account Id' must not be empty.", result.Errors[0].Message);
    }

    [Theory]
    [InlineData(-0.00001)]
    [InlineData(100.1)]
    public async Task CannotCall_AddAccount_WithInvalid_InterestRate_ReturnsFailed(decimal value)
    {
        var command = new AdjustInterestRate.Command(
            "1",
            ObjectId.GenerateNewId().ToString(),
            value);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal($"'Interest Rate' must be between 0 and 100. You entered {value}.",
            result.Errors[0].Message);
    }
}