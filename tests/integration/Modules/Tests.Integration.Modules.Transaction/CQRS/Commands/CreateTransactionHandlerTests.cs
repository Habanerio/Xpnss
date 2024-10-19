using Habanerio.Xpnss.Modules.Transactions.Common;
using Habanerio.Xpnss.Modules.Transactions.CQRS.Commands;
using Habanerio.Xpnss.Modules.Transactions.DTOs;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using MongoDB.Bson;
using Tests.Integration.Common;
using Xunit.Abstractions;

namespace Tests.Integration.Modules.Transaction.CQRS.Commands;

[Collection(nameof(TransactionsMongoCollection))]
public class CreateTransactionHandlerTests : IClassFixture<TransactionsTestDbContextFixture>//, IDisposable
{
    private readonly ITestOutputHelper _outputHelper;

    private readonly ITransactionsRepository _transactionsRepository;

    private readonly TestTransactionsRepository _verifyRepository;

    private readonly CreateTransaction.Handler _testHandler;

    private readonly string _userId = "1";

    public CreateTransactionHandlerTests(TransactionsTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _transactionsRepository = dbContextFixture.TransactionsRepository;

        _verifyRepository = dbContextFixture.VerifyTransactionsRepository;

        _testHandler = new CreateTransaction.Handler(_transactionsRepository);
    }

    [Fact]
    public void Can_Instantiate_Handler()
    {
        var handler = new CreateTransaction.Handler(_transactionsRepository);

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
    public async Task CanCall_Handle_CreateTransaction()
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
            new()
            {
                Amount = 200,
                CategoryId = ObjectId.GenerateNewId().ToString(),
                Description = "Another Item Description"
            }
        };

        var expectedAccountId = ObjectId.GenerateNewId().ToString();
        var expectedTransactionDateTime = DateTime.Now;
        var expectedMerchant = new MerchantDto
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "Test Merchant",
            Location = "Test Location"
        };
        var expectedDescription = "Test PURCHASE Transaction";

        var command = new CreateTransaction.Command(
            _userId,
            expectedAccountId,
            expectedItems,
            expectedTransactionDateTime,
            TransactionTypes.PURCHASE.ToString(),
            expectedDescription,
            expectedMerchant
        );

        // Act
        var result = await _testHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);
        var transactionId = ObjectId.Parse(result.Value.Id);
        Assert.NotEqual(ObjectId.Empty, transactionId);

        var actualTransaction = await _verifyRepository.GetAsync(transactionId);

        Assert.NotNull(actualTransaction);
        Assert.Equal(_userId, actualTransaction.UserId);
        Assert.Equal(ObjectId.Parse(expectedAccountId), actualTransaction.AccountId);
        Assert.Equal(expectedTransactionDateTime.Date, actualTransaction.TransactionDate.Date);
        Assert.Equal(TransactionTypes.PURCHASE, actualTransaction.TransactionTypes);
        Assert.Equal(expectedDescription, actualTransaction.Description);
        Assert.Equal(expectedItems.Count, actualTransaction.Items.Count);
        Assert.Equal(actualTransaction.TotalAmount, actualTransaction.Items.Sum(i => i.Amount));
        Assert.Equal(actualTransaction.TotalAmount, actualTransaction.TotalOwing);

        Assert.NotNull(actualTransaction.Merchant);
        Assert.Equal(expectedMerchant.Id, actualTransaction.Merchant.Id.ToString());
        Assert.Equal(expectedMerchant.Name, actualTransaction.Merchant.Name);
        Assert.Equal(expectedMerchant.Location, actualTransaction.Merchant.Location);
    }

    /// <summary>
    /// Tests that a new Merchant is created and an Id is generated when the Merchant Id is empty.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CanCall_Handle_CreateTransaction_NewMerchant()
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
            new()
            {
                Amount = 200,
                CategoryId = ObjectId.GenerateNewId().ToString(),
                Description = "Another Item Description"
            }
        };

        var expectedAccountId = ObjectId.GenerateNewId().ToString();
        var expectedTransactionDateTime = DateTime.Now;
        var expectedMerchant = new MerchantDto
        {
            Id = "",
            Name = "Test Merchant",
            Location = "Test Location"
        };
        var expectedDescription = "Test PURCHASE Transaction";

        var command = new CreateTransaction.Command(
            _userId,
            expectedAccountId,
            expectedItems,
            expectedTransactionDateTime,
            TransactionTypes.PURCHASE.ToString(),
            expectedDescription,
            expectedMerchant
        );

        // Act
        var result = await _testHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);
        var transactionId = ObjectId.Parse(result.Value.Id);
        Assert.NotEqual(ObjectId.Empty, transactionId);

        var actualTransaction = await _verifyRepository.GetAsync(transactionId);

        Assert.NotNull(actualTransaction.Merchant);
        Assert.False(actualTransaction.Merchant.Id.Equals(ObjectId.Empty));
        Assert.Equal(expectedMerchant.Name, actualTransaction.Merchant.Name);
        Assert.Equal(expectedMerchant.Location, actualTransaction.Merchant.Location);
    }
}