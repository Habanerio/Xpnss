using Habanerio.Xpnss.Modules.Transactions.CQRS.Queries;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using Tests.Integration.Common;
using Xunit.Abstractions;

namespace Tests.Integration.Modules.Transaction.CQRS.Queries;

[Collection(nameof(TransactionsMongoCollection))]
public class GetTransactionsHandlerTests : IClassFixture<TransactionsTestDbContextFixture>//, IDisposable
{
    private readonly ITestOutputHelper _outputHelper;

    private readonly List<(string UserId, string TransactionId, string AccountId)> _actualTransactions;

    private readonly ITransactionsRepository _transactionsRepository;

    private readonly TestTransactionsRepository _verifyRepository;

    private readonly GetTransactions.Handler _testHandler;

    private readonly string _userId = "1";

    public GetTransactionsHandlerTests(TransactionsTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _actualTransactions = dbContextFixture.ActualTransactions;

        _transactionsRepository = dbContextFixture.TransactionsRepository;

        _testHandler = new GetTransactions.Handler(_transactionsRepository);
    }

    [Fact]
    public void Can_Instantiate_Handler()
    {
        var handler = new GetTransactions.Handler(_transactionsRepository);

        Assert.NotNull(handler);
    }

    [Fact]
    public void Cannot_Instantiate_Handler_WithNull_Repository_ThrowsException()
    {
        ITransactionsRepository repository = null;

        var error = Assert.Throws<ArgumentNullException>(() =>
            new GetTransactions.Handler(repository));

        Assert.Equal("Value cannot be null. (Parameter 'repository')", error.Message);
    }

    [Fact]
    public async Task CanCall_Handle_GetTransactionsByUserId()
    {
        Dictionary<string, int> userTransactions = _actualTransactions.Where(x => x.UserId == _userId)
            .GroupBy(x => x.UserId)
            .ToDictionary(x => x.Key, x => x.Count());

        foreach (var userTransaction in userTransactions)
        {
            // Act
            var query = new GetTransactions.Query(
                userTransaction.Key,
                FromDate: DateTime.UtcNow.AddYears(-1));

            var results = await _testHandler
                .Handle(query, CancellationToken.None);

            // Assert
            Assert.True(results.IsSuccess);
            Assert.NotNull(results.Value);

            var actualTransactions = results.Value;
            Assert.NotNull(actualTransactions);
            Assert.NotEmpty(actualTransactions);
            Assert.Equal(userTransaction.Value, actualTransactions.Count());
        }
    }

    [Fact]
    public async Task CanCall_Handle_GetTransactionsByAccountId()
    {
        var userAccountIds = _actualTransactions.Where(x => x.UserId == _userId)
            .Select(x => x.AccountId)
            .Distinct()
            .ToList();


        foreach (var userAccountId in userAccountIds)
        {
            // Act
            var query = new GetTransactions.Query(
                _userId,
                userAccountId,
                FromDate: DateTime.UtcNow.AddYears(-1));

            var results = await _testHandler.Handle(
                query, CancellationToken.None);

            // Assert
            Assert.True(results.IsSuccess);
            Assert.NotNull(results.Value);

            var actualTransactions = results.Value;
            Assert.NotNull(actualTransactions);
            Assert.NotEmpty(actualTransactions);
        }
    }
}