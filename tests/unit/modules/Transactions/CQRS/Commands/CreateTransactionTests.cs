using FluentResults;
using Habanerio.Xpnss.Modules.Transactions.Common;
using Habanerio.Xpnss.Modules.Transactions.CQRS.Commands;
using Habanerio.Xpnss.Modules.Transactions.Data;
using Habanerio.Xpnss.Modules.Transactions.DTOs;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using MongoDB.Bson;
using Moq;

namespace Habanerio.Xpnss.Tests.Unit.Modules.Transactions.CQRS.Commands;

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
        var expectedUserId = "test-user-id";
        var expectedAccountId = ObjectId.GenerateNewId();
        var expectedTransactionDate = DateTime.UtcNow;
        var expectedDescription = "Some description";
        var expectedMerchant = new MerchantDto
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "Merchant Name",
            Location = "Merchant Location"
        };

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
            expectedUserId,
            expectedAccountId.ToString(),
            expectedItems,
            expectedTransactionDate,
            TransactionTypes.CHARGE.ToString(),
            expectedDescription,
            expectedMerchant
        );

        var expectedDocument = new TransactionDocument()
        {
            Id = ObjectId.GenerateNewId(),
            AccountId = expectedAccountId,
            UserId = expectedUserId,
            TransactionDate = expectedTransactionDate,
            TransactionTypes = TransactionTypes.CHARGE,
            Items = expectedItems.Select(i =>
                new TransactionItem(
                    ObjectId.GenerateNewId(),
                    i.Amount,
                    i.Description,
                    !string.IsNullOrWhiteSpace(i.CategoryId) ? ObjectId.Parse(i.CategoryId) : ObjectId.Empty
                )).ToList(),
        };

        _repository.Setup(x =>
                x.AddAsync(It.IsAny<TransactionDocument>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(expectedDocument));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(!string.IsNullOrWhiteSpace(result.Value.Id));
        Assert.Equal(expectedDocument.Id.ToString(), result.Value.Id);
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
            DateTime.Now,
            TransactionTypes.CHARGE.ToString(),
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
            DateTime.Now,
            TransactionTypes.CHARGE.ToString(),
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
            DateTime.Now,
            TransactionTypes.CHARGE.ToString(),
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