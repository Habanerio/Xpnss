using Habanerio.Xpnss.Modules.Transactions.CQRS.Queries;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using Moq;

namespace Transactions.CQRS.Queries;

public class GetTransactionsByUserIdTests
{
    private readonly Mock<ITransactionsRepository> _repository;

    private readonly GetTransactions.Handler _handler;

    public GetTransactionsByUserIdTests()
    {
        _repository = new Mock<ITransactionsRepository>();

        _handler = new GetTransactions.Handler(_repository.Object);
    }

    [Fact]
    public void Can_Instantiate_Handler()
    {
        var handler = new GetTransactions.Handler(_repository.Object);

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
}