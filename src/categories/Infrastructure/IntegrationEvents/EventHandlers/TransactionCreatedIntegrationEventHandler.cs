////TODO: Need to know if it's a credit or debit transaction. Don't have that without querying for the account first.

//using Habanerio.Xpnss.Categories.Domain.Interfaces;
//using Habanerio.Xpnss.Shared.Types;
//using Habanerio.Xpnss.Shared.ValueObjects;
//using Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;
//using Habanerio.Xpnss.MonthlyTotals.Domain.Interfaces;
//using Microsoft.Extensions.Logging;

//namespace Habanerio.Xpnss.Categories.Infrastructure.IntegrationEvents;

///// <summary>
///// Responsible for handling the 'TransactionCreatedIntegrationEvent'
///// and updating the Category's MonthlyTotal.
///// </summary>
///// <param name="categoriesRepository"></param>
///// <param name="categoryMonthlyTotalsRepository"></param>
///// <param name="logger"></param>
//public class TransactionCreatedIntegrationEventHandler(
//    ICategoriesRepository categoriesRepository,
//    IMonthlyTotalsRepository categoryMonthlyTotalsRepository,
//    ILogger<TransactionCreatedIntegrationEventHandler> logger) :
//    IIntegrationEventHandler<TransactionCreatedIntegrationEvent>
//{
//    private readonly ICategoriesRepository _categoriesRepository = categoriesRepository ??
//        throw new ArgumentNullException(nameof(categoriesRepository));

//    private readonly ICategoryMonthlyTotalsRepository _categoryMonthlyTotalsRepository = categoryMonthlyTotalsRepository ??
//        throw new ArgumentNullException(nameof(categoryMonthlyTotalsRepository));

//    private readonly ILogger<TransactionCreatedIntegrationEventHandler> _logger = logger ??
//        throw new ArgumentNullException(nameof(logger));

//    public async Task Handle(
//        TransactionCreatedIntegrationEvent @event,
//        CancellationToken cancellationToken)
//    {
//        try
//        {
//            await UpdateMonthlyTotalAsync(@event, cancellationToken);

//            _logger.LogInformation(@event.Id.ToString(),
//                "A '{@transactionType}' Transaction {@transactionId} was added to Account {@accountId}",
//                @event.TransactionType,
//                @event.TransactionId,
//                @event.AccountId);
//        }
//        catch (Exception e)
//        {
//            _logger.LogError(e, "An error occurred while trying to update Account '{@accountId}' for User '{@userId}'", @event.AccountId, @event.UserId);

//            throw;
//        }
//    }

//    private async Task UpdateMonthlyTotalAsync(
//        TransactionCreatedIntegrationEvent @event,
//        CancellationToken cancellationToken = default)
//    {
//        var isCreditTransaction = TransactionEnums.DoesBalanceIncrease(account.AccountType, @event.TransactionType);

//        var monthlyTotalResult = await _categoryMonthlyTotalsRepository
//            .GetAsync(
//                @event.UserId,
//                @event.AccountId,
//                @event.DateOfTransaction.Year,
//                @event.DateOfTransaction.Month,
//                cancellationToken);

//        if (monthlyTotalResult.IsFailed)
//            throw new InvalidOperationException(monthlyTotalResult.Errors[0]?.Message ??
//                $"An error occurred while trying to retrieve Account Monthly Total '{@event.AccountId}' for User '{@event.UserId}'");

//        var existingMonthlyTotal = monthlyTotalResult.ValueOrDefault;

//        if (existingMonthlyTotal is null)
//        {
//            // Should the Account itself be responsible for creating something for the 'Monthly Total'?
//            existingMonthlyTotal = AccountMonthlyTotal.New(
//                new AccountId(@event.AccountId),
//                new UserId(@event.UserId),
//                @event.DateOfTransaction.Year,
//                @event.DateOfTransaction.Month,
//                isCreditTransaction,
//                new Money(@event.Amount));
//        }
//        else
//        {
//            // May want to add business logic to the MonthlyTotal entity to do the updates
//            if (isCreditTransaction)
//            {
//                existingMonthlyTotal.CreditCount += 1;
//                existingMonthlyTotal.CreditTotalAmount += new Money(@event.Amount);
//            }
//            else
//            {
//                existingMonthlyTotal.DebitCount += 1;
//                existingMonthlyTotal.DebitTotalAmount += new Money(@event.Amount);
//            }
//        }

//        try
//        {
//            var rslt = await _categoryMonthlyTotalsRepository.AddAsync(existingMonthlyTotal, cancellationToken);

//            if (rslt.IsFailed)
//                throw new InvalidOperationException(rslt.Errors[0]?.Message ??
//                    "An error occurred while trying to update the Account Monthly Total");
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine(e);

//            throw;
//        }
//    }
//}