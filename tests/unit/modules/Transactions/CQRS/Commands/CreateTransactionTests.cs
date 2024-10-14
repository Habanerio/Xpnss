using FluentResults;
using Habanerio.Xpnss.Modules.Transactions.Common;
using Habanerio.Xpnss.Modules.Transactions.CQRS.Commands;
using Habanerio.Xpnss.Modules.Transactions.Data;
using Habanerio.Xpnss.Modules.Transactions.DTOs;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using MongoDB.Bson;
using Moq;

namespace Transactions.CQRS.Commands;

public class CreateTransactionTests
{
    private readonly Mock<ITransactionsRepository> _repository;

    private readonly CreateTransaction.Handler _handler;

    public CreateTransactionTests()
    {
        _repository = new Mock<ITransactionsRepository>();

        _handler = new CreateTransaction.Handler(_repository.Object);
    }

    [Fact]
    public void Can_Instantiate_Handler()
    {
        var handler = new CreateTransaction.Handler(_repository.Object);

        Assert.NotNull(handler);
    }

    [Fact]
    public void Cannot_Instantiate_Handler_WithNull_Repository_ThrowsException()
    {
        ITransactionsRepository repository = null;

        var error = Assert.Throws<ArgumentNullException>(() =>
            new CreateTransaction.Handler(repository));

        Assert.Equal("Value cannot be null. (Parameter 'repository')", error.Message);
    }

    [Fact]
    public async Task CanCall_CreateTransaction()
    {
        // Arrange
        var expectedItems = new List<TransactionItemDto>
        {
            new()
            {
                Amount = 500,
                CategoryId = "",
                Description = ""
            },
            new()
            {
                Amount = 100,
                CategoryId = ObjectId.GenerateNewId().ToString(),
                Description = "Some Item Description"
            },
        };

        var command = new CreateTransaction.Command(
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            expectedItems,
            DateTimeOffset.Now,
            TransactionTypes.CHARGE,
            "Some description",
            new MerchantDto
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Merchant Name",
                Location = "Merchant Location"
            }
        );

        _repository.Setup(x =>
                x.AddAsync(It.IsAny<TransactionDocument>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ObjectId.GenerateNewId()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(!string.IsNullOrWhiteSpace(result.Value));
    }

    [Fact]
    public async Task CannotCall_CreateTransaction_WithNull_Request_ThrowsException()
    {
        CreateTransaction.Command? command = null;

        var error = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Cannot pass null model to Validate. (Parameter 'instanceToValidate')", error.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_CreateTransaction_WithInvalid_UserId_ReturnsFailed(string? value)
    {
        var command = new CreateTransaction.Command(
            value,
            ObjectId.GenerateNewId().ToString(),
            new List<TransactionItemDto>
            {
                new ()
                {
                    Amount = 500,
                    CategoryId = "",
                    Description = ""
                }
            },
            DateTimeOffset.Now,
            TransactionTypes.CHARGE,
            "Some description",
            new MerchantDto
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Merchant Name",
                Location = "Merchant Location"
            }
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("'User Id' must not be empty.", result.Errors[0].Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_CreateTransaction_WithInvalid_AccountId_ReturnsFailed(string? value)
    {
        var command = new CreateTransaction.Command(
            ObjectId.GenerateNewId().ToString(),
            value,
            new List<TransactionItemDto>
            {
                new ()
                {
                    Amount = 500,
                    CategoryId = "",
                    Description = ""
                }
            },
            DateTimeOffset.Now,
            TransactionTypes.CHARGE,
            "Some description",
            new MerchantDto
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Merchant Name",
                Location = "Merchant Location"
            }
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("'Account Id' must not be empty.", result.Errors[0].Message);
    }

    [Fact]
    public async Task CannotCall_CreateTransaction_WithEmpty_Items_ReturnsFailed()
    {
        var command = new CreateTransaction.Command(
            ObjectId.GenerateNewId().ToString(),
            ObjectId.GenerateNewId().ToString(),
            new List<TransactionItemDto>(),
            DateTimeOffset.Now,
            TransactionTypes.CHARGE,
            "Some description",
            new MerchantDto
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Merchant Name",
                Location = "Merchant Location"
            }
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("'Items' must not be empty.", result.Errors[0].Message);
    }
}