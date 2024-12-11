//using Habanerio.Xpnss.Shared.Types;
//using Habanerio.Xpnss.Shared.ValueObjects;
//using Habanerio.Xpnss.Infrastructure.IntegrationEvents;
//using Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;
//using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
//using Microsoft.Extensions.Logging;

//namespace Habanerio.Xpnss.PayerPayees.Infrastructure.IntegrationEvents.EventHandlers;

///// <summary>
///// Responsible for handling the 'TransactionCreatedIntegrationEvent'
///// and updating the Account's Balance and the MonthlyTotal.
///// </summary>
///// <param name="accountsRepository"></param>
///// <param name="accountMonthlyTotalsRepository"></param>
///// <param name="logger"></param>
//public class TransactionCreatedIntegrationEventHandler(
//    IPayerPayeesRepository payerPayeesRepository,
//    IPayerPayeeTotalsRepository accountMonthlyTotalsRepository,
//    //IClientSessionHandle mongoSession,
//    ILogger<TransactionCreatedIntegrationEventHandler> logger) :
//    IIntegrationEventHandler<TransactionCreatedIntegrationEvent>
//{
//    private readonly IAccountsRepository _accountsRepository = accountsRepository ??
//        throw new ArgumentNullException(nameof(accountsRepository));

//    private readonly IAccountMonthlyTotalsRepository _accountMonthlyTotalsRepository = accountMonthlyTotalsRepository ??
//        throw new ArgumentNullException(nameof(accountMonthlyTotalsRepository));

//    //private readonly IClientSessionHandle _mongoSession = mongoSession ??
//    //    throw new ArgumentNullException(nameof(mongoSession));

//    private readonly ILogger<TransactionCreatedIntegrationEventHandler> _logger = logger ??
//        throw new ArgumentNullException(nameof(logger));

//    public async Task Handle(
//        TransactionCreatedIntegrationEvent @event,
//        CancellationToken cancellationToken)
//    {
//        // I want to wrap this in a transaction, but apparently, 'Standalone servers do not support transactions.'
//        //using (_mongoSession)
//        //{
//        //    _mongoSession.StartTransaction();

//        try
//        {
//            // Maybe publish internal Domain Events to handle each one?
//            var ofxAccount = await UpdateAccountBalanceAsync(@event, cancellationToken);

//            //TODO: Finish MonthlyTotals
//            await UpdateMonthlyTotalAsync(@event, ofxAccount, cancellationToken);

//            //      await _mongoSession.CommitTransactionAsync(cancellationToken);

//            _logger.LogInformation(@event.Id.ToString(),
//                "A '{@transactionType}' Transaction {@transactionId} was added to Account {@accountId}",
//                @event.TransactionType,
//                @event.TransactionId,
//                @event.AccountId);
//        }
//        catch (Exception e)
//        {
//            _logger.LogError(e, "An error occurred while trying to update Account '{@accountId}' for User '{@userId}'", @event.AccountId, @event.UserId);

//            //        await _mongoSession.AbortTransactionAsync(cancellationToken);

//            throw;
//        }
//        //}
//    }

//    private async Task<BaseAccount> UpdateAccountBalanceAsync(
//        TransactionCreatedIntegrationEvent @event,
//        CancellationToken cancellationToken = default)
//    {
//        var accountResult = await _accountsRepository.GetAsync(@event.UserId, @event.AccountId, cancellationToken);

//        if (accountResult.IsFailed)
//            throw new InvalidOperationException(accountResult.Errors[0]?.Message ??
//                $"An error occurred while trying to retrieve Account '{@event.AccountId}' for User '{@event.UserId}'");

//        if (accountResult.Value is null)
//            throw new InvalidOperationException(
//                $"Account '{@event.AccountId}' could not be found for user '{@event.UserId}'");

//        var ofxAccount = accountResult.Value;

//        ofxAccount.ApplyTransactionAmount(new Money(@event.Amount), @event.TransactionType);

//        try
//        {
//            var updateResult = await _accountsRepository.UpdateAsync(ofxAccount, cancellationToken);

//            return updateResult.IsSuccess ?
//                ofxAccount :
//                throw new InvalidOperationException(updateResult.Errors[0]?.Message ??
//                    "An error occurred while trying to update the Account");
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine(e);

//            throw;
//        }
//    }

//    private async Task UpdateMonthlyTotalAsync(
//        TransactionCreatedIntegrationEvent @event,
//        BaseAccount ofxAccount,
//        CancellationToken cancellationToken = default)
//    {
//        var isCreditTransaction = TransactionEnums.DoesBalanceIncrease(ofxAccount.AccountType, @event.TransactionType);

//        var monthlyTotalResult = await _accountMonthlyTotalsRepository
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
//            var rslt = await _accountMonthlyTotalsRepository.AddAsync(existingMonthlyTotal, cancellationToken);

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